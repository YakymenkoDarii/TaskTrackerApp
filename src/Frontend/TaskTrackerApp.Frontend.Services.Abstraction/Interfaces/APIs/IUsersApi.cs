using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;

public interface IUsersApi
{
    [Get("/api/Users/search")]
    Task<IApiResponse<Result<IEnumerable<UserSummaryDto>>>> SearchUsersAsync([AliasAs("term")] string searchTerm, CancellationToken cancellationToken);
}