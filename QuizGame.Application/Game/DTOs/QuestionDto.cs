namespace QuizGame.Application.Game.DTOs;

public record QuestionDto(Guid QuestionId, string Text, string[] Choices, string ProviderId);