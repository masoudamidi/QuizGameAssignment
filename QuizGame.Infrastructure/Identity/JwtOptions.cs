namespace QuizGame.Infrastructure.Identity;

public record JwtOptions(string Secret, string Issuer, string Audience, int ExpiryMinutes);