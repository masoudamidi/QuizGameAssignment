using QuizGame.Domain.GameSessions;

namespace QuizGame.Application.Game.Interfaces;

public interface IGameSessionRepository
{
    Task<GameSession?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(GameSession session, CancellationToken ct = default);
    Task<IReadOnlyList<GameSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}