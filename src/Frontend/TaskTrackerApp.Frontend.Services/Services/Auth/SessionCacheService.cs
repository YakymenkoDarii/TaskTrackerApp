using Microsoft.Extensions.Caching.Memory;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class SessionCacheService : ISessionCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(2);
    public readonly Guid InstanceId = Guid.NewGuid();
    public string? CurrentSessionId { get; set; }

    public SessionCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public string CreateSession(AuthUserDataDto userData)
    {
        var sessionId = Guid.NewGuid().ToString();
        _memoryCache.Set(sessionId, userData, _sessionTimeout);
        return sessionId;
    }

    public AuthUserDataDto? GetSessionData(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId)) return null;

        if (_memoryCache.TryGetValue(sessionId, out AuthUserDataDto? data))
        {
            return data;
        }
        return null;
    }

    public void RemoveSession(string sessionId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            _memoryCache.Remove(sessionId);
        }
    }
}