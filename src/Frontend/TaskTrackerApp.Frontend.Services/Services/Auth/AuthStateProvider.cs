using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ISessionCacheService _sessionCache;

    public AuthStateProvider(ISessionCacheService sessionCache)
    {
        _sessionCache = sessionCache;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessionId = _sessionCache.CurrentSessionId;

        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.FromResult(anonymous);
        }

        var userData = _sessionCache.GetSessionData(sessionId);

        if (userData == null || string.IsNullOrWhiteSpace(userData.AccessToken))
        {
            return Task.FromResult(anonymous);
        }

        var claims = JwtParser.ParseClaimsFromJwt(userData.AccessToken);

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    public void NotifyAuthStatusChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}