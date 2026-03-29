using System.Security.Claims;
using QuizGame.Application.Game.DTOs;
using QuizGame.Application.Game.Services;

namespace QuizGame.Api.Endpoints;
 
public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games").WithTags("Game").RequireAuthorization();
 
        group.MapPost("/", async (ClaimsPrincipal user, GameService svc, CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            try
            {
                var result = await svc.StartAsync(userId, ct);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Problem(ex.Message, statusCode: 503);
            }
        });
 
        group.MapPost("/{sessionId:guid}/answer", async (
            Guid sessionId,
            SubmitAnswerRequest req,
            ClaimsPrincipal user,
            GameService svc,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            try
            {
                var result = await svc.SubmitAnswerAsync(sessionId, userId, req, ct);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException) { return Results.NotFound(); }
            catch (UnauthorizedAccessException) { return Results.Forbid(); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
        });
 
        // group.MapGet("/history", async (ClaimsPrincipal user, GameService svc, CancellationToken ct) =>
        // {
        //     var history = await svc.GetHistoryAsync(user.GetUserId(), ct);
        //     return Results.Ok(history);
        // });
        //
        // group.MapGet("/history/export", async (ClaimsPrincipal user, GameService svc, CancellationToken ct) =>
        // {
        //     var history = await svc.GetHistoryAsync(user.GetUserId(), ct);
        //     var csv = ToCsv(history);
        //     return Results.File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "game-history.csv");
        // });
        //
        // app.MapGet("/leaderboard", async (GameService svc, CancellationToken ct) =>
        // {
        //     var board = await svc.GetLeaderboardAsync(ct);
        //     return Results.Ok(board);
        // }).WithTags("Game");
    }
 
    // private static string ToCsv(IReadOnlyList<Application.DTOs.GameHistoryItem> items)
    // {
    //     var sb = new System.Text.StringBuilder();
    //     sb.AppendLine("SessionId,StartedAt,EndedAt,FinalScore,Status,QuestionsAnswered");
    //     foreach (var i in items)
    //         sb.AppendLine($"{i.SessionId},{i.StartedAt:O},{i.EndedAt:O},{i.FinalScore},{i.Status},{i.QuestionsAnswered}");
    //     return sb.ToString();
    // }
}
 
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim missing."));
 
    // Minimal helper for DI in avatar endpoint
    public class UserIdAccessor(ClaimsPrincipal principal)
    {
        public Guid Value => principal.GetUserId();
    }
}