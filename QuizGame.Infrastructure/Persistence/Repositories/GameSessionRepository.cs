using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Domain.GameSessions;

namespace QuizGame.Infrastructure.Persistence.Repositories;

public class GameSessionRepository(AppDbContext db) : IGameSessionRepository
{
    public Task<GameSession?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.GameSessions.Include(s => s.Attempts).FirstOrDefaultAsync(s => s.Id == id, ct);
 
    public async Task AddAsync(GameSession session, CancellationToken ct = default) =>
        await db.GameSessions.AddAsync(session, ct);
 
    public async Task<IReadOnlyList<GameSession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await db.GameSessions
            .Include(s => s.Attempts)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync(ct);
 
    public Task SaveAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}