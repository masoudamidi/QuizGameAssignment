using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationInfo(
    [property: JsonPropertyName("station_id")] string StationId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon,
    [property: JsonPropertyName("capacity")] int? Capacity);