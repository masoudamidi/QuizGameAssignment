using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Game.Interfaces;

public interface IStationCache
{
    IReadOnlyList<StationSnapshot> GetAll();
    void Update(IEnumerable<StationSnapshot> snapshots);
}