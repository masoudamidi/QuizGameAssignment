using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.GameSessions;

public class GameSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Score { get; private set; } 
    public GameStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public ICollection<Attempt> Attempts { get; private set; } = new List<Attempt>();
 
    private GameSession() { }
 
    public static GameSession Start(Guid userId, int durationSeconds = 60)
    {
        var now = DateTime.UtcNow;
        return new GameSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Score = 0,
            Status = GameStatus.InProgress,
            StartedAt = now,
            ExpiresAt = now.AddSeconds(durationSeconds),
        };
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
 
    public (bool correct, int scoreDelta) SubmitAnswer(Guid questionId, string chosenAnswer, string correctAnswer)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game session is not in progress.");
        if (IsExpired)
        {
            End();
            throw new InvalidOperationException("Game session has expired.");
        }
 
        var isCorrect = string.Equals(chosenAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);
        var delta = isCorrect ? ScoreConstants.CorrectPoints : ScoreConstants.WrongPoints;
 
        Score += delta;
 
        Attempts.Add(Attempt.Create(Id, questionId, chosenAnswer, isCorrect, delta, Score));
        return (isCorrect, delta);
    }
 
    public void End()
    {
        if (Status != GameStatus.InProgress) return;
        EndedAt = DateTime.UtcNow;
        Status = Score >= 0 ? GameStatus.Won : GameStatus.Lost;
    }
}