using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs;

public class GbfsSyncService(IServiceScopeFactory scopeFactory, IStationCache stationCache, ILogger<GbfsSyncService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Initial load
        await SyncAsync(ct);
 
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
        while (!ct.IsCancellationRequested && await timer.WaitForNextTickAsync(ct))
            await SyncAsync(ct);
    }
    
    private async Task SyncAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var clients = scope.ServiceProvider.GetRequiredService<IEnumerable<IGbfsClient>>();
 
        var tasks = clients.Select(c => FetchSafe(c, ct));
        var results = await Task.WhenAll(tasks);
        var all = results.SelectMany(r => r).ToList();
 
        if (all.Count > 0)
        {
            stationCache.Update(all);
            logger.LogInformation("GBFS sync: {Count} stations from {Providers} providers",
                all.Count, results.Count(r => r.Count > 0));
        }
    }
    
    private async Task<IReadOnlyList<StationSnapshot>> FetchSafe(IGbfsClient client, CancellationToken ct)
    {
        try
        {
            return await client.FetchSnapshotsAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GBFS fetch failed for {Provider}", client.ProviderId);
            return [];
        }
    }
}