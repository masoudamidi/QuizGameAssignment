using QuizGame.Application.Game.DTOs;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Domain.GameSessions;
using QuizGame.Domain.Questions;

namespace QuizGame.Application.Game.Services;

public class GameService(
    IGameSessionRepository sessions,
    IStationCache stationCache,
    QuestionGenerator questionGenerator) : IGameService
{
    // Per-session question queues (in-memory; fine for a single-instance deployment)
    private static readonly Dictionary<Guid, Queue<Question>> _sessionQueues = new();
    private static readonly object _lock = new();
 
    public async Task<StartGameResponse> StartAsync(Guid userId, CancellationToken ct = default)
    {
        var stations = stationCache.GetAll();
        if (stations.Count < 4)
            throw new InvalidOperationException("GBFS data not yet loaded. Try again in a moment.");
 
        var questions = questionGenerator.Generate(stations, count: 30);
        var session = GameSession.Start(userId);
 
        await sessions.AddAsync(session, ct);
        await sessions.SaveAsync(ct);
 
        var queue = new Queue<Question>(questions);
        lock (_lock) _sessionQueues[session.Id] = queue;
 
        var first = Dequeue(session.Id)!;
        return new StartGameResponse(session.Id, ToDto(first), session.ExpiresAt);
    }
 
    public async Task<SubmitAnswerResponse> SubmitAnswerAsync(
        Guid sessionId, Guid userId, SubmitAnswerRequest req, CancellationToken ct = default)
    {
        var session = await sessions.FindByIdAsync(sessionId, ct)
            ?? throw new KeyNotFoundException("Session not found.");
 
        if (session.UserId != userId)
            throw new UnauthorizedAccessException();
 
        // Need the current question to validate — peek at it from domain perspective
        var currentQuestion = PeekCurrent(sessionId)
            ?? throw new InvalidOperationException("No more questions in this session.");
 
        var (correct, delta) = session.SubmitAnswer(currentQuestion.Id, req.ChosenAnswer, currentQuestion.CorrectAnswer);
 
        // Advance queue
        Dequeue(sessionId);
 
        var next = Dequeue(sessionId);
        bool gameOver = next == null || session.IsExpired;
 
        if (gameOver) session.End();
 
        await sessions.SaveAsync(ct);
 
        return new SubmitAnswerResponse(
            correct,
            delta,
            session.Score,
            next != null && !gameOver ? ToDto(next) : null,
            gameOver,
            gameOver ? session.Status.ToString() : null
        );
    }
    
    public async Task<IReadOnlyList<GameHistoryItem>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
    {
        var userSessions = await sessions.GetByUserIdAsync(userId, ct);
        return userSessions.Select(s => new GameHistoryItem(
            s.Id,
            s.StartedAt,
            s.EndedAt,
            s.Score,
            s.Status.ToString(),
            s.Attempts.Count
        )).ToList();
    }
 
    private static QuestionDto ToDto(Question q) =>
        new(q.Id, q.Text, q.Choices, q.ProviderId);
 
    private static Question? Dequeue(Guid sessionId)
    {
        lock (_lock)
        {
            if (_sessionQueues.TryGetValue(sessionId, out var q) && q.Count > 0)
                return q.Dequeue();
            return null;
        }
    }
 
    private static Question? PeekCurrent(Guid sessionId)
    {
        lock (_lock)
        {
            if (_sessionQueues.TryGetValue(sessionId, out var q) && q.Count > 0)
                return q.Peek();
            return null;
        }
    }
}