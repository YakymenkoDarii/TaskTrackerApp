using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Requests;
using TaskTrackerApp.Frontend.Domain.DTOs.Auth.Responses;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);

    Task<Result> SignupAsync(SignupRequest signupRequest);

    Task<Result<LoginResponse>> RefreshAsync();

    Task LogoutAsync();
}