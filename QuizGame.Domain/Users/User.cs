using QuizGame.Domain.GameSessions;

namespace QuizGame.Domain.Users;

/// <summary>
/// Entity responsible for all user data and actions.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; private set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; private set; }
    
    public ICollection<GameSession> QuizSessions { get; private set; } = new List<GameSession>();
 
    public static User Create(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
 
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}