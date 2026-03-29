using Microsoft.AspNetCore.Identity;
using QuizGame.Application.Auth.DTOs;
using QuizGame.Application.Auth.Interfaces;
using QuizGame.Domain.Users;

namespace QuizGame.Application.Auth.Services;

public class AuthService(IUserRepository users, ITokenService tokens) : IAuthService
{
    private readonly PasswordHasher<User> _hasher = new();
 
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var existing = await users.FindByEmailAsync(req.Email, ct);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");
 
        var user = User.Create(req.Username, req.Email, "placeholder");
        var hash = _hasher.HashPassword(user, req.Password);
        user = User.Create(req.Username, req.Email, hash);
 
        await users.AddAsync(user, ct);
        await users.SaveAsync(ct);
 
        return new AuthResponse(tokens.GenerateToken(user), user.Email);
    }
 
    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var user = await users.FindByEmailAsync(req.Email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");
 
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");
 
        return new AuthResponse(tokens.GenerateToken(user), user.Email);
    }
}