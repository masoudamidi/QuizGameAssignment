using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizGame.Domain.GameSessions;
using QuizGame.Domain.Users;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<Attempt> Attempts => Set<Attempt>();
 
    protected override void OnModelCreating(ModelBuilder m)
    {
        // User
        m.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256);
        });
 
        // GameSession
        m.Entity<GameSession>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Score);
            e.Property(s => s.Status).HasConversion<string>();
            e.HasMany(s => s.Attempts).WithOne().HasForeignKey(a => a.GameSessionId);
        });
 
        // Attempt
        m.Entity<Attempt>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).ValueGeneratedOnAdd();
            e.Property(a => a.ChosenAnswer).HasMaxLength(512);
        });
    }
}