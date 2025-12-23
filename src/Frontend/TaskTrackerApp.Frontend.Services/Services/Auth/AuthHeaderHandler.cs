using System.Net.Http.Headers;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ISessionCacheService _sessionCacheService;

    public AuthHeaderHandler(ISessionCacheService sessionCacheService)
    {
        _sessionCacheService = sessionCacheService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sessionId = _sessionCacheService.CurrentSessionId;

        // DEBUG
        Console.WriteLine($"[AuthHeaderHandler] Cache Instance: {((SessionCacheService)_sessionCacheService).InstanceId}");
        Console.WriteLine($"[AuthHeaderHandler] Processing request to {request.RequestUri}");
        Console.WriteLine($"[AuthHeaderHandler] Current Session ID: {sessionId ?? "NULL"}");

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            var userData = _sessionCacheService.GetSessionData(sessionId);

            if (userData != null && !string.IsNullOrWhiteSpace(userData.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userData.AccessToken);
                Console.WriteLine("[AuthHeaderHandler] Token attached successfully.");
            }
            else
            {
                Console.WriteLine("[AuthHeaderHandler] Session found, but Token is missing.");
            }
        }
        else
        {
            Console.WriteLine("[AuthHeaderHandler] No Session ID found. Request sent anonymously.");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}