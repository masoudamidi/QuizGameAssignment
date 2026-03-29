using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationInfoRoot(
    [property: JsonPropertyName("data")] StationInfoData Data);