using QuizGame.Application.Auth.DTOs;
using QuizGame.Application.Auth.Services;

namespace QuizGame.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");
 
        group.MapPost("/register", async (RegisterRequest req, AuthService svc, CancellationToken ct) =>
        {
            try
            {
                var result = await svc.RegisterAsync(req, ct);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        });
 
        group.MapPost("/login", async (LoginRequest req, AuthService svc, CancellationToken ct) =>
        {
            try
            {
                var result = await svc.LoginAsync(req, ct);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        });
    }
}