using TaskTrackerApp.Frontend.Domain.DTOs.Auth;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface ISessionCacheService
{
    string? CurrentSessionId { get; set; }

    string CreateSession(AuthUserDataDto userData);

    AuthUserDataDto? GetSessionData(string sessionId);

    void RemoveSession(string sessionId);
}