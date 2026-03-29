namespace QuizGame.Domain.ValueObjects;

public record Score
{
    public const int CorrectPoints = 50;
    public const int IncorrectPoints = -20;
    
    public int Value { get; set; }
    
    private Score(int value) =>  Value = value;

    public static Score Zero => new(0);
    public static Score From(int value) => new (value);
    
    public Score Apply(int delta) => new (Value + delta);
}