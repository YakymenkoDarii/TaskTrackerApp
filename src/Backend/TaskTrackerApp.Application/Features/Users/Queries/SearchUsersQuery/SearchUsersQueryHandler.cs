using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Users.Queries.SearchUsersQuery;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, Result<IEnumerable<UserSummaryDto>>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public SearchUsersQueryHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<UserSummaryDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<IEnumerable<UserSummaryDto>>.Success(new List<UserSummaryDto>());
        }

        using var uow = _uowFactory.Create();

        var users = await uow.UserRepository.SearchAsync(request.SearchTerm, request.ExcludeBoard);

        var userDtos = users.Select(u => new UserSummaryDto
        {
            Id = u.Id,
            DisplayName = u.DisplayName,
            Tag = u.Tag,
            AvatarUrl = u.AvatarUrl,
            Email = u.Email,
        });

        return Result<IEnumerable<UserSummaryDto>>.Success(userDtos);
    }
}