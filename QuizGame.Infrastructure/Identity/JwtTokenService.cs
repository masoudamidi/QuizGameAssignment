using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizGame.Application.Auth.Interfaces;
using QuizGame.Domain.Users;

namespace QuizGame.Infrastructure.Identity;

public class JwtTokenService(IOptions<JwtOptions> opts) : ITokenService
{
    public string GenerateToken(User user)
    {
        var o = opts.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
 
        var token = new JwtSecurityToken(o.Issuer, o.Audience, claims,
            expires: DateTime.UtcNow.AddMinutes(o.ExpiryMinutes),
            signingCredentials: creds);
 
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}