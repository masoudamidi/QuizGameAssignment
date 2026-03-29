using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Game.Interfaces;

public interface IGbfsClient
{
    string ProviderId { get; }
    Task<IReadOnlyList<StationSnapshot>> FetchSnapshotsAsync(CancellationToken ct = default);
}