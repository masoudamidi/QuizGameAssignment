using QuizGame.Application.Auth.DTOs;

namespace QuizGame.Application.Auth.Interfaces;

public interface IAuthService
{
    public Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default);
    public Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default);
}