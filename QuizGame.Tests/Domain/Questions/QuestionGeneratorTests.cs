using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Tests.Domain.Questions;

public class QuestionGeneratorTests
{
    [Fact]
    public void Generate_ShouldThrow_WhenLessThanFourStations()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations().Take(3).ToList();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sut.Generate(stations));
    }

    [Fact]
    public void Generate_ShouldReturnQuestions()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations();

        // Act
        var result = sut.Generate(stations, count: 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.True(result.Count <= 10);
    }

    [Fact]
    public void GeneratedQuestions_ShouldHaveValidStructure()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations();

        // Act
        var result = sut.Generate(stations, count: 10);

        // Assert
        foreach (var q in result)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Text));
            Assert.NotNull(q.Choices);
            Assert.True(q.Choices.Length >= 2);
        }
    }

    [Fact]
    public void EachQuestion_ShouldContainCorrectAnswer()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations();

        // Act
        var result = sut.Generate(stations, count: 10);

        // Assert
        foreach (var q in result)
        {
            Assert.Contains(q.CorrectAnswer, q.Choices);
        }
    }

    [Fact]
    public void Questions_ShouldHaveValidProviderIds()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations(providerId: "P1", city: "CityA");

        // Act
        var result = sut.Generate(stations, count: 5);

        // Assert
        foreach (var q in result)
        {
            Assert.Equal("P1", q.ProviderId);
        }
    }

    [Fact]
    public void BikeCountQuestions_ShouldHaveNumericChoices()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations();

        // Act
        var result = sut.Generate(stations, count: 20);

        var bikeQuestions = result
            .Where(q => q.Text.Contains("How many bikes"))
            .ToList();

        // Assert
        foreach (var q in bikeQuestions)
        {
            Assert.All(q.Choices, choice =>
                Assert.True(int.TryParse(choice, out _)));
        }
    }

    [Fact]
    public void EmptyStationsQuestions_ShouldHaveNumericChoices()
    {
        // Arrange
        var sut = CreateSut();
        var stations = CreateSampleStations();

        // Act
        var result = sut.Generate(stations, count: 20);

        var emptyQuestions = result
            .Where(q => q.Text.Contains("empty"))
            .ToList();

        // Assert
        foreach (var q in emptyQuestions)
        {
            Assert.All(q.Choices, choice =>
                Assert.True(int.TryParse(choice, out _)));
        }
    }

    [Fact]
    public void LargestProviderQuestions_ShouldContainCorrectCity()
    {
        // Arrange
        var sut = CreateSut();

        var stations = new List<StationSnapshot>
        {
            CreateStation("P1", "CityA", 10, 100),
            CreateStation("P1", "CityA", 5, 100),

            CreateStation("P2", "CityB", 20, 200),
            CreateStation("P2", "CityB", 15, 200),
        };

        // Act
        var result = sut.Generate(stations, count: 10);

        var providerQuestions = result
            .Where(q => q.Text.Contains("largest total"))
            .ToList();

        // Assert
        foreach (var q in providerQuestions)
        {
            Assert.Contains(q.CorrectAnswer, q.Choices);
        }
    }
    
    private QuestionGenerator CreateSut() => new QuestionGenerator();

    private List<StationSnapshot> CreateSampleStations(string providerId = "P1", string city = "TestCity")
    {
        var stations = new List<StationSnapshot>();

        for (int i = 1; i <= 10; i++)
        {
            stations.Add(new StationSnapshot(
                StationId: i.ToString(),
                Name: $"Station {i}",
                ProviderId: providerId,
                City: city,
                Latitude: 0,
                Longitude: 0,
                BikesAvailable: i,
                DocksAvailable: 20 - i,
                Capacity: 20,
                FetchedAt: DateTime.UtcNow
            ));
        }

        return stations;
    }

    private StationSnapshot CreateStation(string providerId, string city, int bikes, int capacity)
    {
        return new StationSnapshot(
            StationId: Guid.NewGuid().ToString(),
            Name: Guid.NewGuid().ToString(),
            ProviderId: providerId,
            City: city,
            Latitude: 0,
            Longitude: 0,
            BikesAvailable: bikes,
            DocksAvailable: 0,
            Capacity: capacity,
            FetchedAt: DateTime.UtcNow
        );
    }
}