namespace QuizGame.Application.Game.DTOs;

public record StartGameResponse(Guid SessionId, QuestionDto Question, DateTime ExpiresAt);