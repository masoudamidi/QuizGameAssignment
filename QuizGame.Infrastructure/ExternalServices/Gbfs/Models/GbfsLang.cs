using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record GbfsLang(
    [property: JsonPropertyName("feeds")] List<GbfsFeed> Feeds);