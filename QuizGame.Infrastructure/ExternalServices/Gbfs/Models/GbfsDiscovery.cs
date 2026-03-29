using System.Text.Json.Serialization;

namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Models;

public record GbfsDiscovery(
    [property: JsonPropertyName("data")] Dictionary<string, GbfsLang> Data);