using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetArchivedBoards;

public class GetArchivedBoardsQueryHandler : IRequestHandler<GetArchivedBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetArchivedBoardsQueryHandler(ICurrentUserService currentUserService, IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetArchivedBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return UserErrors.NotFound;
        }

        var members = await uow.BoardMembersRepository.GetArchivedByUserIdAsync(userId.Value);

        var dtos = members.Select(m => new BoardDto
        {
            Id = m.Board.Id,
            Title = m.Board.Title,
            Description = m.Board.Description,
            IsArchived = m.Board.IsArchived,
        });

        return dtos.ToList();
    }
}