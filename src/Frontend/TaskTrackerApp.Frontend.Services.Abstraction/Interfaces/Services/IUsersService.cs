using Microsoft.AspNetCore.Components.Forms;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IUsersService
{
    Task<Result> ChangePassword(ChangePasswordRequest request);

    Task<Result<MyProfileDto>> GetProfileAsync();

    Task<Result<IEnumerable<UserSummaryDto>>> SearchUsersAsync(string searchTerm, CancellationToken token, int? excludeBoardId = null);

    Task<Result> UpdateAsync(UpdateUserDto userDto);

    Task<Result<Uri>> UpdateAvatarAsync(IBrowserFile file);
}