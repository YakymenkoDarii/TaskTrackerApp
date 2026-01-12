using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.Users;
using TaskTrackerApp.Frontend.Domain.Results;

namespace TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

public interface IUsersService
{
    Task<Result<IEnumerable<UserSummaryDto>>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken);
}