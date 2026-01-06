using Blazored.SessionStorage;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class TokenStorage : ITokenStorage
{
    private readonly ISyncSessionStorageService _sessionStorage;

    public TokenStorage(ISyncSessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public string? GetAccessToken() => _sessionStorage.GetItem<string>("authToken");

    public void SetAccessToken(string token) => _sessionStorage.SetItem("authToken", token);

    public void ClearAccessToken() => _sessionStorage.RemoveItem("authToken");
}