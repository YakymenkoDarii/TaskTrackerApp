using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;

public interface IAuthApi
{
    [Post("/api/Auth/signup")]
    Task<IApiResponse<AuthResponse?>> SignupAsync([Body] SignupRequest request);

    [Post("/api/Auth/login")]
    Task<IApiResponse<AuthResponse?>> LoginAsync([Body] LoginRequest request);

    [Post("/api/Auth/refresh")]
    Task<IApiResponse<AuthResponse?>> RefreshAsync([Body] RefreshRequest request);
}