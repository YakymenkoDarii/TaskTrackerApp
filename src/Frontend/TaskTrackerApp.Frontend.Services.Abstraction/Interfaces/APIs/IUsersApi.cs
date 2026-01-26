using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IUsersApi
{
    [Get("/api/Users/search")]
    Task<IApiResponse<Result<IEnumerable<UserSummaryDto>>>> SearchUsersAsync([AliasAs("term")] string searchTerm, int? excludeBoardId = null, CancellationToken cancellationToken = default);

    [Put("/api/Users/update")]
    Task<IApiResponse<Result>> UpdateAsync([Body] UpdateUserDto dto);

    [Multipart]
    [Put("/api/Users/update-avatar")]
    Task<IApiResponse<Result<Uri>>> UpdateAvatarAsync([AliasAs("file")] StreamPart file);

    [Put("/api/Users/change-password")]
    Task<IApiResponse<Result>> ChangePassword(ChangePasswordRequest request);

    [Get("/api/Users/me")]
    Task<IApiResponse<Result<MyProfileDto>>> GetProfileAsync();
}