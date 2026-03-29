namespace QuizGame.Application.Game.DTOs;

public record GameHistoryItem(Guid SessionId, DateTime StartedAt, DateTime? EndedAt, int FinalScore, string Status, int QuestionsAnswered);