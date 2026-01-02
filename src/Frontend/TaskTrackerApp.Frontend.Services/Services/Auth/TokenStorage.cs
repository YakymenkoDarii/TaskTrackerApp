using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class TokenStorage : ITokenStorage
{
    private string? _accessToken;

    public string? GetAccessToken() => _accessToken;

    public void SetAccessToken(string token) => _accessToken = token;

    public void ClearAccessToken() => _accessToken = null;
}