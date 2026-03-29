using QuizGame.Domain.GameSessions;

namespace QuizGame.Tests.Domain.GameSessions;

public class AttemptTests
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var chosen = "A";
        var correct = true;
        var delta = 10;
        var running = 50;

        var before = DateTime.UtcNow;

        // Act
        var attempt = Attempt.Create(sessionId, questionId, chosen, correct, delta, running);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, attempt.Id);
        Assert.Equal(sessionId, attempt.GameSessionId);
        Assert.Equal(questionId, attempt.QuestionId);
        Assert.Equal(chosen, attempt.ChosenAnswer);
        Assert.Equal(correct, attempt.IsCorrect);
        Assert.Equal(delta, attempt.ScoreDelta);
        Assert.Equal(running, attempt.RunningScore);

        Assert.True(attempt.AnsweredAt >= before);
        Assert.True(attempt.AnsweredAt <= after);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        // Act
        var attempt1 = Attempt.Create(sessionId, questionId, "A", true, 1, 1);
        var attempt2 = Attempt.Create(sessionId, questionId, "B", false, 0, 1);

        // Assert
        Assert.NotEqual(attempt1.Id, attempt2.Id);
    }
}