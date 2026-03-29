using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Auth.Interfaces;
using QuizGame.Domain.Users;

namespace QuizGame.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
 
    public Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Users.FindAsync([id], ct).AsTask();
 
    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);
 
    public Task SaveAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}