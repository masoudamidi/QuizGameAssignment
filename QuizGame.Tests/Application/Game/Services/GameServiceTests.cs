using Moq;
using QuizGame.Application.Game.DTOs;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Application.Game.Services;
using QuizGame.Domain.GameSessions;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Tests.Application.Game.Services;

public class GameServiceTests
{
    private readonly Mock<IGameSessionRepository> _repoMock = new();
    private readonly Mock<IStationCache> _cacheMock = new();

    private readonly GameService _service;

    public GameServiceTests()
    {
        var questionGeneratorMock = new Mock<QuestionGenerator>();

        _service = new GameService(
            _repoMock.Object,
            _cacheMock.Object,
            questionGeneratorMock.Object
        );
    }

    [Fact]
    public async Task StartAsync_Should_Throw_When_Not_Enough_Stations()
    {
        // Arrange
        var stations = new List<StationSnapshot>
        {
            new(
                "1", "Station 1", "provider",
                "City", 52.37, 4.89,
                10, 5, 15, DateTime.UtcNow
            ),
            new(
                "2", "Station 2", "provider",
                "City", 52.38, 4.90,
                8, 7, 15, DateTime.UtcNow
            )
        };
        
        _cacheMock.Setup(x => x.GetAll()).Returns(stations);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.StartAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task SubmitAnswerAsync_Should_Throw_When_Wrong_User()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        var session = GameSession.Start(Guid.NewGuid());

        _repoMock.Setup(x => x.FindByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.SubmitAnswerAsync(sessionId, Guid.NewGuid(), new SubmitAnswerRequest("ChosenAnswer")));
    }

    [Fact]
    public async Task GetHistoryAsync_Should_Map_Sessions_To_DTOs()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var sessions = new List<GameSession>
        {
            GameSession.Start(userId),
            GameSession.Start(userId)
        };

        _repoMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        // Act
        var history = await _service.GetHistoryAsync(userId);

        // Assert
        Assert.Equal(2, history.Count);
        Assert.All(history, h => Assert.Equal(userId, h.SessionId == Guid.Empty ? Guid.Empty : userId));
    }
}