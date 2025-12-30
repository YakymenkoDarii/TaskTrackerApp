using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IAuthApi
{
    [Post("/api/Auth/signup")]
    Task<IApiResponse> SignupAsync([Body] SignupRequest request);

    [Post("/api/Auth/login")]
    Task<IApiResponse> LoginAsync([Body] LoginRequest request);

    [Post("/api/Auth/logout")]
    Task<IApiResponse> LogoutAsync();

    [Get("/api/Auth/me")]
    Task<IApiResponse<MeDto>> MeAsync();
}