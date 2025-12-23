using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Refit;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ISessionCacheService _sessionCache;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(IAuthApi authApi,
        ISessionCacheService sessionCache,
        ProtectedLocalStorage localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _authApi = authApi;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _sessionCache = sessionCache;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await _authApi.LoginAsync(loginRequest);
            var result = ParseResponse(response);

            if (result.IsSuccess)
            {
                await StartUserSessionAsync(result.Value);
            }

            return result;
        }
        catch (ApiException)
        {
            return Result<AuthResponse>.Failure(new Error("Client.Network", "Network error."));
        }
    }

    public async Task<Result<AuthResponse>> SignupAsync(SignupRequest signupRequest)
    {
        try
        {
            var response = await _authApi.SignupAsync(signupRequest);
            var result = ParseResponse(response);

            if (result.IsSuccess)
            {
                await StartUserSessionAsync(result.Value);
            }

            return result;
        }
        catch (ApiException)
        {
            return Result<AuthResponse>.Failure(new Error("Client.Network", "Network error."));
        }
    }

    public async Task LogoutAsync()
    {
        if (!string.IsNullOrEmpty(_sessionCache.CurrentSessionId))
        {
            _sessionCache.RemoveSession(_sessionCache.CurrentSessionId);
            _sessionCache.CurrentSessionId = null;
        }

        await _localStorage.DeleteAsync("auth_token");

        ((AuthStateProvider)_authStateProvider).NotifyAuthStatusChanged();
    }

    private async Task StartUserSessionAsync(AuthResponse authResponse)
    {
        var userData = new AuthUserDataDto { AccessToken = authResponse.AccessToken };
        var sessionId = _sessionCache.CreateSession(userData);

        _sessionCache.CurrentSessionId = sessionId;
        await _localStorage.SetAsync("auth_token", authResponse.AccessToken);

        ((AuthStateProvider)_authStateProvider).NotifyAuthStatusChanged();
    }

    private Result<AuthResponse> ParseResponse(IApiResponse<AuthResponse> response)
    {
        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            return Result<AuthResponse>.Success(response.Content);
        }

        if (response.Error?.Content is not null)
        {
            var backendError = JsonSerializer.Deserialize<Error>(
                response.Error.Content,
                _jsonOptions);

            if (backendError is not null)
            {
                return Result<AuthResponse>.Failure(backendError);
            }
        }

        return Result<AuthResponse>.Failure(
            new Error("Client.Server", response.ReasonPhrase ?? "Unknown server error"));
    }
}