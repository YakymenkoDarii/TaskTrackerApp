using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IUsersService
{
    Task<Result<IEnumerable<UserSummaryDto>>> SearchUsersAsync(string searchTerm, CancellationToken token, int? excludeBoardId = null);
}