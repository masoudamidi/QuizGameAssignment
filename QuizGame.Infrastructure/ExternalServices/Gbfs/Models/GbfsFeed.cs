using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record GbfsFeed(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url);