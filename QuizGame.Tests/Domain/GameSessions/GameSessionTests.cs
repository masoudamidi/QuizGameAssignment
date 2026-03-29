using QuizGame.Domain.GameSessions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Tests.Domain.GameSessions;

public class GameSessionTests
{
    [Fact]
    public void Start_ShouldInitializeSessionCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var before = DateTime.UtcNow;

        // Act
        var session = GameSession.Start(userId, durationSeconds: 60);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.Equal(userId, session.UserId);
        Assert.Equal(GameStatus.InProgress, session.Status);
        Assert.Equal(Score.Zero.Value, session.Score.Value);

        Assert.True(session.StartedAt >= before);
        Assert.True(session.StartedAt <= after);

        Assert.Equal(session.StartedAt.AddSeconds(60), session.ExpiresAt, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SubmitAnswer_WhenCorrect_ShouldIncreaseScore()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid());
        var questionId = Guid.NewGuid();

        var correctAnswer = "A";

        // Act
        var (isCorrect, delta) = session.SubmitAnswer(questionId, "A", correctAnswer);

        // Assert
        Assert.True(isCorrect);
        Assert.Equal(Score.CorrectPoints, delta);
        Assert.Equal(Score.CorrectPoints, session.Score.Value);
        Assert.Single(session.Attempts);
    }
    
    [Fact]
    public void SubmitAnswer_WhenIncorrect_ShouldDecreaseOrApplyNegativeScore()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid());
        var questionId = Guid.NewGuid();

        // Act
        var (isCorrect, delta) = session.SubmitAnswer(questionId, "B", "A");

        // Assert
        Assert.False(isCorrect);
        Assert.Equal(Score.IncorrectPoints, delta);
        Assert.Equal(Score.IncorrectPoints, session.Score.Value);
    }
    
    [Fact]
    public void SubmitAnswer_WhenSessionNotInProgress_ShouldThrow()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid());
        session.End();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            session.SubmitAnswer(Guid.NewGuid(), "A", "A"));

        Assert.Equal("Game session is not in progress.", ex.Message);
    }
    
    [Fact]
    public void SubmitAnswer_WhenExpired_ShouldEndSessionAndThrow()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid(), durationSeconds: 0);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            session.SubmitAnswer(Guid.NewGuid(), "A", "A"));

        Assert.Equal("Game session has expired.", ex.Message);
        Assert.NotNull(session.EndedAt);
        Assert.NotEqual(GameStatus.InProgress, session.Status);
    }
    
    [Fact]
    public void End_ShouldSetStatusToWon_WhenScoreIsPositive()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid());
        session.SubmitAnswer(Guid.NewGuid(), "A", "A");

        // Act
        session.End();

        // Assert
        Assert.Equal(GameStatus.Won, session.Status);
        Assert.NotNull(session.EndedAt);
    }
    
    [Fact]
    public void End_ShouldSetStatusToLost_WhenScoreIsNegative()
    {
        // Arrange
        var session = GameSession.Start(Guid.NewGuid());
        session.SubmitAnswer(Guid.NewGuid(), "B", "A");

        // Act
        session.End();

        // Assert
        Assert.Equal(GameStatus.Lost, session.Status);
    }
}