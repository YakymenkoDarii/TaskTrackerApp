using Microsoft.AspNetCore.Components.Authorization;
using Refit;
using System.Security.Claims;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthApi _authApi;

    private ClaimsPrincipal _anonymous =
        new(new ClaimsIdentity());

    private ClaimsPrincipal _currentUser;

    public AuthStateProvider(IAuthApi authApi)
    {
        _authApi = authApi;
        _currentUser = _anonymous;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _authApi.MeAsync();

            if (!response.IsSuccessStatusCode || response.Content is null)
                return new AuthenticationState(_anonymous);

            var user = response.Content;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            _currentUser = new ClaimsPrincipal(identity);

            return new AuthenticationState(_currentUser);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserLoggedIn()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void NotifyUserLoggedOut()
    {
        _currentUser = _anonymous;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}