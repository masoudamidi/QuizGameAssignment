using QuizGame.Domain.Users;

namespace QuizGame.Application.Auth.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}