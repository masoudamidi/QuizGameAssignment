using QuizGame.Application.Game.DTOs;

namespace QuizGame.Application.Game.Interfaces;

public interface IGameService
{
    public Task<StartGameResponse> StartAsync(Guid userId, CancellationToken ct = default);

    public Task<SubmitAnswerResponse> SubmitAnswerAsync(Guid sessionId, Guid userId, SubmitAnswerRequest req, CancellationToken ct = default);
}