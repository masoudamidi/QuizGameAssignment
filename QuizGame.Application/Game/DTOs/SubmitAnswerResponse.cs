namespace QuizGame.Application.Game.DTOs;

public record SubmitAnswerResponse(bool Correct, int ScoreDelta, int TotalScore, QuestionDto? NextQuestion, bool GameOver, string? GameResult);