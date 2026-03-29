using System.Text.Json;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs;

public abstract class GbfsClient(HttpClient http, string discoveryUrl) : IGbfsClient
{
    public abstract string ProviderId { get; }
    protected abstract string City { get; }
 
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
 
    public async Task<IReadOnlyList<StationSnapshot>> FetchSnapshotsAsync(CancellationToken ct = default)
    {
        var (infoUrl, statusUrl) = await DiscoverFeedUrlsAsync(ct);
        var infoTask = FetchAsync<StationInfoRoot>(infoUrl, ct);
        var statusTask = FetchAsync<StationStatusRoot>(statusUrl, ct);
        await Task.WhenAll(infoTask, statusTask);
 
        var infoMap = infoTask.Result.Data.Stations.ToDictionary(s => s.StationId);
        var statusMap = statusTask.Result.Data.Stations.ToDictionary(s => s.StationId);
 
        var now = DateTime.UtcNow;
        return infoMap.Keys
            .Where(id => statusMap.ContainsKey(id))
            .Select(id =>
            {
                var info = infoMap[id];
                var status = statusMap[id];
                return new StationSnapshot(
                    id, 
                    info.Name, 
                    ProviderId, 
                    City,
                    info.Lat, 
                    info.Lon,
                    status.NumBikesAvailable,
                    status.NumDocksAvailable,
                    info.Capacity ?? (status.NumBikesAvailable + status.NumDocksAvailable),
                    now);
            })
            .ToList();
    }
 
    private async Task<(string infoUrl, string statusUrl)> DiscoverFeedUrlsAsync(CancellationToken ct)
    {
        var discovery = await FetchAsync<GbfsDiscovery>(discoveryUrl, ct);
        var feeds = discovery.Data.TryGetValue("en", out var lang)
            ? lang.Feeds
            : discovery.Data.Values.FirstOrDefault()?.Feeds
              ?? throw new InvalidOperationException($"No feeds in GBFS discovery for {ProviderId}");
 
        var infoUrl = feeds.First(f => f.Name == "station_information").Url;
        var statusUrl = feeds.First(f => f.Name == "station_status").Url;
        return (infoUrl, statusUrl);
    }
 
    private async Task<T> FetchAsync<T>(string url, CancellationToken ct)
    {
        var response = await http.GetStringAsync(url, ct);
        return JsonSerializer.Deserialize<T>(response, _json)
            ?? throw new InvalidDataException($"Empty response from {url}");
    }
}