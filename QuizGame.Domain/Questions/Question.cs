namespace QuizGame.Domain.Questions;

public record Question(
    Guid     Id,
    string   Text,
    string[] Choices,
    string   CorrectAnswer,
    string   ProviderId);