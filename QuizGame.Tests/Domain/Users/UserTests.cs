using QuizGame.Domain.Users;

namespace QuizGame.Tests.Domain.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldInitializeUserCorrectly()
    {
        // Arrange
        var username = "masoud";
        var email = "masoud@masoud.Com";
        var passwordHash = "hashed_password";

        var before = DateTime.UtcNow;

        // Act
        var user = User.Create(username, email, passwordHash);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal("masoud@masoud.com", user.Email); // normalized
        Assert.Equal(passwordHash, user.PasswordHash);

        Assert.True(user.CreatedAt >= before);
        Assert.True(user.CreatedAt <= after);

        Assert.NotNull(user.QuizSessions);
        Assert.Empty(user.QuizSessions);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenUsernameIsInvalid(string invalidUsername)
    {
        // Arrange
        var email = "test@example.com";
        var passwordHash = "hash";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            User.Create(invalidUsername, email, passwordHash));

        Assert.Equal("Username is required.", ex.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenEmailIsInvalid(string invalidEmail)
    {
        // Arrange
        var username = "masoud";
        var passwordHash = "hash";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            User.Create(username, invalidEmail, passwordHash));

        Assert.Equal("Email is required.", ex.Message);
    }
    
    [Fact]
    public void Create_ShouldNormalizeEmailToLowerInvariant()
    {
        // Arrange
        var user = User.Create(
            "masoud",
            "masoud@masoud.COM",
            "hash"
        );

        // Assert
        Assert.Equal("masoud@masoud.com", user.Email);
    }
}