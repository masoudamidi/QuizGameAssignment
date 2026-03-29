using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationStatus(
    [property: JsonPropertyName("station_id")] string StationId,
    [property: JsonPropertyName("num_bikes_available")] int NumBikesAvailable,
    [property: JsonPropertyName("num_docks_available")] int NumDocksAvailable);