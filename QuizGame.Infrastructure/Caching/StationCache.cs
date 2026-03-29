using Microsoft.Extensions.Caching.Memory;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.Caching;

public class StationCache(IMemoryCache cache) : IStationCache
{
    private const string Key = "stations";
 
    public IReadOnlyList<StationSnapshot> GetAll() =>
        cache.TryGetValue(Key, out List<StationSnapshot>? list) ? list! : [];
 
    public void Update(IEnumerable<StationSnapshot> snapshots)
    {
        var list = snapshots.ToList();
        cache.Set(Key, list, TimeSpan.FromMinutes(5));
    }
}