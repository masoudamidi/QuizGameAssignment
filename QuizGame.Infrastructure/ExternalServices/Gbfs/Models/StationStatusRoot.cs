using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationStatusRoot(
    [property: JsonPropertyName("data")] StationStatusData Data);