using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Settings;

namespace TaskTrackerApp.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string CreateAccessToken(User user, out DateTime expiration)
    {
        expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenValidityMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Tag),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string RefreshToken, DateTime Expiration) CreateRefreshToken()
    {
        var expiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays);

        return (Guid.NewGuid().ToString(), expiration);
    }
}