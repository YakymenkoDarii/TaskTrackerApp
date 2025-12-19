using Refit;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;

namespace TaskTrackerApp.Frontend.Services;

public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthService(IAuthApi authApi)
    {
        _authApi = authApi;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await _authApi.LoginAsync(loginRequest);
            return ParseResponse(response);
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
            return ParseResponse(response);
        }
        catch (ApiException)
        {
            return Result<AuthResponse>.Failure(new Error("Client.Network", "Network error."));
        }
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