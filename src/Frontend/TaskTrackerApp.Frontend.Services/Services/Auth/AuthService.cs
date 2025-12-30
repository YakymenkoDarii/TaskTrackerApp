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

    private readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public AuthService(IAuthApi authApi)
    {
        _authApi = authApi;
    }

    public async Task<Result> LoginAsync(LoginRequest request)
    {
        var result = await _authApi.LoginAsync(request);
        return ParseResponse(result);
    }

    public async Task<Result> SignupAsync(SignupRequest request)
    {
        var result = await _authApi.SignupAsync(request);
        return ParseResponse(result);
    }

    public async Task<Result> LogoutAsync()
    {
        var result = await _authApi.LogoutAsync();
        return ParseResponse(result);
    }

    public async Task<Result<MeDto>> GetMeAsync()
    {
        try
        {
            var response = await _authApi.MeAsync();
            return response.IsSuccessStatusCode ? response.Content : null;
        }
        catch
        {
            return null;
        }
    }

    private Result ParseResponse(IApiResponse response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        if (response.Error?.Content is not null)
        {
            var backendError = JsonSerializer.Deserialize<Error>(response.Error.Content, _jsonOptions);
            if (backendError is not null)
            {
                return Result<AuthUserDataDto>.Failure(backendError);
            }
        }
        return Result<AuthUserDataDto>
            .Failure(new Error(ClientErrors.NetworkError.Code,
            response.ReasonPhrase ?? ClientErrors.UnknownNetworkError.Message));
    }
}