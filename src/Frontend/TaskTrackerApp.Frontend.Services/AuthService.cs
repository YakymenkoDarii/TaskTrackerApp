using TaskTrackerApp.Frontend.Domain.DTOs.Auth;
using TaskTrackerApp.Frontend.Domain.Result;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;

namespace TaskTrackerApp.Frontend.Services;

public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;

    public AuthService(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var response = await _authApi.LoginAsync(loginRequest);

        if (!response.IsSuccessful
            || response.Error != null
            || response.Content is null)
        {
            var errorMessage =
                 response.Error?.Message
                 ?? response.Error?.Content
                 ?? "Login failed";

            return Result<AuthResponse>.Failure(errorMessage);
        }

        var content = response.Content;

        if (string.IsNullOrEmpty(content.RefreshToken)
            || string.IsNullOrEmpty(content.AccessToken))
        {
            return Result<AuthResponse>.Failure("Token is empty");
        }

        return Result<AuthResponse>.Success(content);
    }

    public async Task<Result<AuthResponse>> SignupAsync(SignupRequest signupRequest)
    {
        var response = await _authApi.SignupAsync(signupRequest);

        if (!response.IsSuccessful
            || response.Error != null
            || response.Content is null)
        {
            var errorMessage =
                 response.Error?.Message
                 ?? response.Error?.Content
                 ?? "Signup failed";

            return Result<AuthResponse>.Failure(errorMessage);
        }

        var content = response.Content;

        if (string.IsNullOrEmpty(content.RefreshToken)
            || string.IsNullOrEmpty(content.AccessToken))
        {
            return Result<AuthResponse>.Failure("Token is empty");
        }

        return Result<AuthResponse>.Success(content);
    }
}