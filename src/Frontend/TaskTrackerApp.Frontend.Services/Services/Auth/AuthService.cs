using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.Text.Json;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly ITokenStorage _tokenStorage;

    private readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public AuthService(
        IAuthApi authApi,
        ITokenStorage tokenStorage)
    {
        _authApi = authApi;
        _tokenStorage = tokenStorage;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var result = await _authApi.LoginAsync(request);

        if (!result.IsSuccessStatusCode || result.Content is null)
        {
            return ParseError(result);
        }

        _tokenStorage.SetAccessToken(result.Content.AccessToken);

        return Result<LoginResponse>.Success(result.Content);
    }

    public async Task<Result<LoginResponse>> RefreshAsync()
    {
        var result = await _authApi.RefreshAsync(new RefreshTokenRequest
        {
            Tag = "silent-refresh"
        });

        if (!result.IsSuccessStatusCode || result.Content is null)
        {
            return Result<LoginResponse>.Failure(
                new Error("Auth.RefreshFailed", "Session expired"));
        }

        _tokenStorage.SetAccessToken(result.Content.AccessToken);

        return Result<LoginResponse>.Success(result.Content);
    }

    public async Task<Result> SignupAsync(SignupRequest request)
    {
        var result = await _authApi.SignupAsync(request);

        if (result.IsSuccessStatusCode)
            return Result.Success();

        if (result.Error?.Content != null)
        {
            var error = JsonSerializer.Deserialize<Error>(
                result.Error.Content, _jsonOptions);

            if (error != null)
                return Result.Failure(error);
        }

        return Result.Failure(
            new Error("NetworkError", result.ReasonPhrase ?? "Unknown error"));
    }

    public async Task LogoutAsync()
    {
        _tokenStorage.ClearAccessToken();
        await _authApi.LogoutAsync();
    }

    private Result<LoginResponse> ParseError(IApiResponse<LoginResponse> response)
    {
        if (response.Error?.Content != null)
        {
            var error = JsonSerializer.Deserialize<Error>(
                response.Error.Content, _jsonOptions);

            if (error != null)
                return Result<LoginResponse>.Failure(error);
        }

        return Result<LoginResponse>.Failure(
            new Error("NetworkError", response.ReasonPhrase ?? "Unknown"));
    }
}