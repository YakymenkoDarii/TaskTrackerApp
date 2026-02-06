using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetArchivedBoards;

public class GetArchivedBoardsQueryHandler : IRequestHandler<GetArchivedBoardsQuery, Result<IEnumerable<ArchivedBoardDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetArchivedBoardsQueryHandler(ICurrentUserService currentUserService, IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<ArchivedBoardDto>>> Handle(GetArchivedBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return UserErrors.NotFound;
        }

        var memberships = (await uow.ArchivedBoardMembersRepository.GetByUserIdAsync(userId.Value)).ToList();
        List<ArchivedBoardDto> dtos = new();

        foreach (var membership in memberships)
        {
            var board = await uow.ArchivedBoardsRepository.GetById(membership.ArchivedBoardId);

            var dto = new ArchivedBoardDto
            {
                Id = board.Id,
                Title = board.Title,
                Description = board.Description,
                CanUnarchive = membership.Role == BoardRole.Admin ? true : false,
            };

            dtos.Add(dto);
        }

        return dtos;
    }
}