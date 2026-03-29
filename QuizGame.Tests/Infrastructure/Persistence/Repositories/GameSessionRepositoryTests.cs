using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.GameSessions;
using QuizGame.Infrastructure.Persistence;
using QuizGame.Infrastructure.Persistence.Repositories;

namespace QuizGame.Tests.Infrastructure.Persistence.Repositories;

public class GameSessionRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistAndFindById()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new GameSessionRepository(db);

        var session = GameSession.Start(Guid.NewGuid());

        // Act
        await repo.AddAsync(session);
        await repo.SaveAsync();

        var result = await repo.FindByIdAsync(session.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(session.Id, result.Id);
        Assert.Equal(session.UserId, result.UserId);
    }
    
    [Fact]
    public async Task FindByIdAsync_ShouldIncludeAttempts()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new GameSessionRepository(db);

        var session = GameSession.Start(Guid.NewGuid());
        
        session.SubmitAnswer(Guid.NewGuid(), "A", "A");

        await repo.AddAsync(session);
        await repo.SaveAsync();

        // Act
        var result = await repo.FindByIdAsync(session.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Attempts);
    }
    
    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnOrderedSessions()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new GameSessionRepository(db);

        var userId = Guid.NewGuid();

        var session1 = GameSession.Start(userId);
        await Task.Delay(10); // ensure different timestamps
        var session2 = GameSession.Start(userId);

        await repo.AddAsync(session1);
        await repo.AddAsync(session2);
        await repo.SaveAsync();

        // Act
        var result = await repo.GetByUserIdAsync(userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.First().StartedAt >= result.Last().StartedAt);
    }
    
    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnOnlyUserSessions()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new GameSessionRepository(db);

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        await repo.AddAsync(GameSession.Start(user1));
        await repo.AddAsync(GameSession.Start(user2));
        await repo.SaveAsync();

        // Act
        var result = await repo.GetByUserIdAsync(user1);

        // Assert
        Assert.Single(result);
        Assert.All(result, s => Assert.Equal(user1, s.UserId));
    }
    
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}