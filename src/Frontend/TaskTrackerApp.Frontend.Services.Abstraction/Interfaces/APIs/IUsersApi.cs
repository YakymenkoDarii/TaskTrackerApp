using Refit;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IUsersApi
{
    [Get("/api/Users/search")]
    Task<IApiResponse<Result<IEnumerable<UserSummaryDto>>>> SearchUsersAsync([AliasAs("term")] string searchTerm, int? excludeBoardId = null, CancellationToken cancellationToken = default);
}