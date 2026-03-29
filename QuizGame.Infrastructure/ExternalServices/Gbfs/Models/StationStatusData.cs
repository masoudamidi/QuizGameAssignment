using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationStatusData(
    [property: JsonPropertyName("stations")] List<StationStatus> Stations);