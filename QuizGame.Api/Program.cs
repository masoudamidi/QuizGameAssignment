using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuizGame.Api.Endpoints;
using QuizGame.Application.Auth.Interfaces;
using QuizGame.Application.Auth.Services;
using QuizGame.Application.Game.Interfaces;
using QuizGame.Application.Game.Services;
using QuizGame.Domain.Questions;
using QuizGame.Infrastructure.Caching;
using QuizGame.Infrastructure.ExternalServices.Gbfs;
using QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;
using QuizGame.Infrastructure.Identity;
using QuizGame.Infrastructure.Persistence;
using QuizGame.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(cfg.GetConnectionString("Default")));

builder.Services.AddMemoryCache();

builder.Services.Configure<JwtOptions>(cfg.GetSection("Jwt"));
var jwtSection = cfg.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.Issuer,
            ValidAudience = jwtSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Secret))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<OsloBysykkelClient>();
builder.Services.AddHttpClient<CitiBikeNycClient>();
builder.Services.AddHttpClient<VelibParisClient>();

builder.Services.AddTransient<IGbfsClient, OsloBysykkelClient>();
builder.Services.AddTransient<IGbfsClient, CitiBikeNycClient>();
builder.Services.AddTransient<IGbfsClient, VelibParisClient>();

builder.Services.AddSingleton<IStationCache, StationCache>();
builder.Services.AddSingleton<QuestionGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GameService>();

builder.Services.AddHostedService<GbfsSyncService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
 
app.MapAuthEndpoints();
app.MapGameEndpoints();

app.Run();