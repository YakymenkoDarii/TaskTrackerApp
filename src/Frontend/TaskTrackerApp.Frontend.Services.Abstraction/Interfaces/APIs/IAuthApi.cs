using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Frontend.Domain.Results;

public interface IAuthApi
{
    [Post("/api/Auth/signup")]
    Task<IApiResponse<Result>> SignupAsync([Body] SignupRequest request);

    [Post("/api/Auth/login")]
    Task<IApiResponse<Result<LoginResponse>>> LoginAsync([Body] LoginRequest request);

    [Post("/api/Auth/refresh")]
    Task<IApiResponse<Result<LoginResponse>>> RefreshAsync([Body] RefreshTokenRequest request);

    [Post("/api/Auth/logout")]
    Task<IApiResponse<Result>> LogoutAsync();
}