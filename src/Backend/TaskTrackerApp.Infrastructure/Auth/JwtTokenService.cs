using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Settings;

namespace TaskTrackerApp.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUnitOfWorkFactory _uowFactory;

    public JwtTokenService(IOptions<JwtSettings> jwtOptions, IUnitOfWorkFactory uowFactory)
    {
        _jwtSettings = jwtOptions.Value;
        _uowFactory = uowFactory;
    }

    public string GenerateAccessToken(User user)
    {
        var secretKey = _jwtSettings.SecretKey;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Tag),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        var randomString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        return new RefreshToken
        {
            Token = randomString,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<AuthResponse>? RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal is null)
        {
            return null;
        }

        var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return null;
        }

        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetAsync(userId);
        var savedRefreshToken = await uow.RefreshTokenRepository.GetByTokenAsync(refreshToken);

        if (savedRefreshToken is null ||
            savedRefreshToken.UserId != userId ||
            savedRefreshToken.ExpiresAt < DateTime.UtcNow ||
            savedRefreshToken.RevokedAt is not null ||
            user is null)
        {
            return null;
        }

        savedRefreshToken.RevokedAt = DateTime.UtcNow;
        await uow.RefreshTokenRepository.UpdateAsync(savedRefreshToken);

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();
        newRefreshToken.UserId = userId;

        await uow.RefreshTokenRepository.AddAsync(newRefreshToken);
        await uow.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var secretKey = _jwtSettings.SecretKey;
        if (string.IsNullOrEmpty(secretKey)) return null;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidAudience = _jwtSettings.Audience,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}