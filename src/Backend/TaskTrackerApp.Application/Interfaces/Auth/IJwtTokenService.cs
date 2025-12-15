using TaskTrackerApp.Domain.DTOs.Auth;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken();

    Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken);
}