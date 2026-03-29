using QuizGame.Application.Game.DTOs;
using QuizGame.Application.Game.Interfaces;

namespace QuizGame.Application.Game.Services;

public class GameService : IGameService
{
    public Task<StartGameResponse> StartAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<SubmitAnswerResponse> SubmitAnswerAsync(Guid sessionId, Guid userId, SubmitAnswerRequest req, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}