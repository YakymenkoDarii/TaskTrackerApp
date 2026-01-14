using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.User;
using TaskTrackerApp.Domain.Entities;
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

        var validUsers = new List<User>();

        if (request.ExcludeBoard.HasValue)
        {
            foreach (var user in users)
            {
                bool isPending = await uow.BoardInvitationsRepository
                    .IsInvitationPendingAsync(request.ExcludeBoard.Value, user.Email);

                if (!isPending)
                {
                    validUsers.Add(user);
                }
            }
        }
        else
        {
            validUsers = users.ToList();
        }

        var userDtos = validUsers.Select(u => new UserSummaryDto
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