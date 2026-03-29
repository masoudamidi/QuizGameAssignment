namespace QuizGame.Domain.GameSessions;

public class Attempt
{
    public Guid Id { get; private set; }
    public Guid GameSessionId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string ChosenAnswer { get; private set; } = default!;
    public bool IsCorrect { get; private set; }
    public int ScoreDelta { get; private set; }
    public int RunningScore { get; private set; }
    public DateTime AnsweredAt { get; private set; }
 
    private Attempt() { }
 
    public static Attempt Create(Guid sessionId, Guid questionId, string chosen, bool correct, int delta, int running) =>
        new()
        {
            Id = Guid.NewGuid(),
            GameSessionId = sessionId,
            QuestionId = questionId,
            ChosenAnswer = chosen,
            IsCorrect = correct,
            ScoreDelta = delta,
            RunningScore = running,
            AnsweredAt = DateTime.UtcNow
        };
}