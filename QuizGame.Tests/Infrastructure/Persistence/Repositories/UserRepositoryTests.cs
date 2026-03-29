using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.Users;
using QuizGame.Infrastructure.Persistence;
using QuizGame.Infrastructure.Persistence.Repositories;

namespace QuizGame.Tests.Infrastructure.Persistence.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistAndFindById()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new UserRepository(db);

        var email = "Test@Email.com";
        var user = new User
        {
            Id = Guid.NewGuid(), 
            Email = email.ToLowerInvariant(),
            PasswordHash = "hashedpassword",
            Username = "Test",
        };

        // Act
        await repo.AddAsync(user);
        await repo.SaveAsync();

        var result = await repo.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }
    
    [Fact]
    public async Task FindByEmailAsync_ShouldReturnUser_IgnoringCase()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new UserRepository(db);

        var email = "Test@Email.com";
        var user = new User
        {
            Id = Guid.NewGuid(), 
            Email = email.ToLowerInvariant(),
            PasswordHash = "hashedpassword",
            Username = "Test",
        };

        await repo.AddAsync(user);
        await repo.SaveAsync();

        // Act
        var result = await repo.FindByEmailAsync("TEST@EMAIL.COM");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }
    
    [Fact]
    public async Task FindByEmailAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new UserRepository(db);

        // Act
        var result = await repo.FindByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAllowFindingByEmail()
    {
        // Arrange
        var db = CreateDbContext();
        var repo = new UserRepository(db);

        var email = "user@test.com";
        var user = new User
        {
            Id = Guid.NewGuid(), 
            Email = email.ToLowerInvariant(),
            PasswordHash = "hashedpassword",
            Username = "Test",
        };

        await repo.AddAsync(user);
        await repo.SaveAsync();

        // Act
        var result = await repo.FindByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email.ToLowerInvariant(), result.Email);
    }
    
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}