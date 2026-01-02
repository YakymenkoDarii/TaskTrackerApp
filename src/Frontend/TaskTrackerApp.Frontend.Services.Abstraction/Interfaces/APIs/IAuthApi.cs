using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Responses;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IAuthApi
{
    [Post("/api/Auth/signup")]
    Task<IApiResponse> SignupAsync([Body] SignupRequest request);

    [Post("/api/Auth/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Post("/api/Auth/refresh")]
    Task<ApiResponse<LoginResponse>> RefreshAsync([Body] RefreshTokenRequest request);

    [Post("/api/Auth/logout")]
    Task<IApiResponse> LogoutAsync();
}