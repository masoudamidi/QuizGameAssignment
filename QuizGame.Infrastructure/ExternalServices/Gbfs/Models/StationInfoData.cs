using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record StationInfoData(
    [property: JsonPropertyName("stations")] List<StationInfo> Stations);