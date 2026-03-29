using Microsoft.AspNetCore.Identity;
using Moq;
using QuizGame.Application.Auth.DTOs;
using QuizGame.Application.Auth.Interfaces;
using QuizGame.Application.Auth.Services;
using QuizGame.Domain.Users;

namespace QuizGame.Tests.Application.Auth.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_AndReturnToken()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var tokenServiceMock = new Mock<ITokenService>();

        var request = new RegisterRequest("testuser", "test@test.com","Password123!");

        userRepoMock
            .Setup(r => r.FindByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        userRepoMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        userRepoMock
            .Setup(r => r.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("fake-token");

        var service = new AuthService(userRepoMock.Object, tokenServiceMock.Object);

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.Equal("fake-token", result.Token);
        Assert.Equal(request.Username, result.Username);

        userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        userRepoMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var tokenServiceMock = new Mock<ITokenService>();

        var existingUser = User.Create("user", "test@test.com", "hash");

        userRepoMock
            .Setup(r => r.FindByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var service = new AuthService(userRepoMock.Object, tokenServiceMock.Object);

        var request = new RegisterRequest("user", "test@test.com", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(request));
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var tokenServiceMock = new Mock<ITokenService>();

        var password = "Password123!";
        var user = User.Create("user", "test@test.com", "hash");

        var hasher = new PasswordHasher<User>();
        var hashed = hasher.HashPassword(user, password);

        user = User.Create("user", "test@test.com", hashed);

        userRepoMock
            .Setup(r => r.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns("token");

        var service = new AuthService(userRepoMock.Object, tokenServiceMock.Object);

        var request = new LoginRequest(user.Email, password);

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.Equal("token", result.Token);
        Assert.Equal(user.Username, result.Username);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var tokenServiceMock = new Mock<ITokenService>();

        userRepoMock
            .Setup(r => r.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = new AuthService(userRepoMock.Object, tokenServiceMock.Object);

        var request = new LoginRequest("test@test.com", "password");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(request));
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var tokenServiceMock = new Mock<ITokenService>();
        
        var hasher = new PasswordHasher<object?>();
        
        var correctPassword = "Password123!";
        var hashedPassword = hasher.HashPassword(null, correctPassword);
        var user = User.Create("user", "test@test.com", hashedPassword);
        

        userRepoMock
            .Setup(r => r.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var service = new AuthService(userRepoMock.Object, tokenServiceMock.Object);

        var request = new LoginRequest(user.Email, "wrong-password");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(request));
    }
}