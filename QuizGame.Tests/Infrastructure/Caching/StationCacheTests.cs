using Microsoft.Extensions.Caching.Memory;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.Caching;

namespace QuizGame.Tests.Infrastructure.Caching;

public class StationCacheTests
{
    [Fact]
    public void GetAll_ShouldReturnEmpty_WhenCacheIsEmpty()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Update_ShouldStoreItems_InCache()
    {
        // Arrange
        var sut = CreateSut();
        var snapshots = new List<StationSnapshot>
        {
            CreateSnapshot("1"),
            CreateSnapshot("2")
        };

        // Act
        sut.Update(snapshots);
        var result = sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.StationId == "1");
        Assert.Contains(result, s => s.StationId == "2");
    }

    [Fact]
    public void GetAll_ShouldReturnSameInstance_AsStored()
    {
        // Arrange
        var sut = CreateSut();
        var snapshots = new List<StationSnapshot>
        {
            CreateSnapshot("1")
        };

        // Act
        sut.Update(snapshots);
        var result = sut.GetAll();

        // Assert
        Assert.Equal(snapshots, result);
    }

    [Fact]
    public void Update_ShouldOverwritePreviousValues()
    {
        // Arrange
        var sut = CreateSut();

        var first = new List<StationSnapshot>
        {
            CreateSnapshot("1")
        };

        var second = new List<StationSnapshot>
        {
            CreateSnapshot("2"),
            CreateSnapshot("3")
        };

        // Act
        sut.Update(first);
        sut.Update(second);
        var result = sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, s => s.StationId == "1");
        Assert.Contains(result, s => s.StationId == "2");
        Assert.Contains(result, s => s.StationId == "3");
    }
    
    private StationCache CreateSut()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        return new StationCache(memoryCache);
    }

    private StationSnapshot CreateSnapshot(string id)
    {
        return new StationSnapshot(
            StationId: id,
            Name: $"Station {id}",
            ProviderId: "P1",
            City: "CityA",
            Latitude: 0,
            Longitude: 0,
            BikesAvailable: 10,
            DocksAvailable: 5,
            Capacity: 15,
            FetchedAt: DateTime.UtcNow
        );
    }
}