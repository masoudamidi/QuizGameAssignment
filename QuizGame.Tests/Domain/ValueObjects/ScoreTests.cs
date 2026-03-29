using FluentAssertions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Tests.Domain.ValueObjects;

public class ScoreTests
{
    [Fact]
    public void Score_starts_at_zero()
    {
        Score.Zero.Value.Should().Be(0);
    }
 
    [Fact]
    public void Correct_answer_adds_50_points()
    {
        // Arrange & Act
        var score = Score.Zero.Apply(Score.CorrectPoints);
        
        // Assert
        score.Value.Should().Be(50);
    }
 
    [Fact]
    public void Wrong_answer_subtracts_20_points()
    {
        // Arrange & Act
        var score = Score.Zero.Apply(Score.IncorrectPoints);
        
        // Assert
        score.Value.Should().Be(-20);
    }
 
    [Fact]
    public void Score_accumulates_correctly_across_multiple_answers()
    {
        // Arrange & Act
        var score = Score.Zero
            .Apply(Score.CorrectPoints)  // 50
            .Apply(Score.CorrectPoints)  // 100
            .Apply(Score.IncorrectPoints)    // 80
            .Apply(Score.IncorrectPoints);   // 60
 
        // Assert
        score.Value.Should().Be(60);
    }
 
    [Fact]
    public void Score_is_immutable()
    {
        // Arrange & Act
        var original = Score.Zero;
        var _ = original.Apply(Score.CorrectPoints);
        
        // Assert
        original.Value.Should().Be(0);
    }
}