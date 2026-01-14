using MediatR;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Queries.SearchUsersQuery;

public class SearchUsersQuery : IRequest<Result<IEnumerable<UserSummaryDto>>>
{
    public string SearchTerm { get; set; }

    public int? ExcludeBoard { get; set; } = null;
}