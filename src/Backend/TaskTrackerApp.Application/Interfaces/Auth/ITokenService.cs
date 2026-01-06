using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.Interfaces.Auth;

public interface ITokenService
{
    string CreateAccessToken(User user, out DateTime expiration);

    (string RefreshToken, DateTime Expiration) CreateRefreshToken();
}