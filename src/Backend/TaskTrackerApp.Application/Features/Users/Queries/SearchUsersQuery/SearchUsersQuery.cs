using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Queries.SearchUsersQuery;

public class SearchUsersQuery : IRequest<Result<IEnumerable<UserSummaryDto>>>
{
    public string SearchTerm { get; set; }
}