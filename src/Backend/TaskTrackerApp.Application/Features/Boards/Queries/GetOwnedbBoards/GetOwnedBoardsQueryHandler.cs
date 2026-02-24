using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.BoardMappers;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetOwnedbBoards;

internal class GetOwnedBoardsQueryHandler : IRequestHandler<GetOwnedBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetOwnedBoardsQueryHandler(
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetOwnedBoardsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return UserErrors.Unauthorized;

        using var uow = _uowFactory.Create();

        bool isUserPro = await uow.UserRepository.IsUserProAsync(userId.Value);
        int limit = isUserPro ? int.MaxValue : 3;

        var memberships = await uow.BoardMembersRepository.GetByUserIdAsync(userId.Value);

        var ownedMemberships = memberships
            .Where(m => m.Board.CreatedById == userId.Value)
            .OrderByDescending(m => m.Board.LastModified)
            .ToList();

        var boardDtos = ownedMemberships
            .Select((member, index) =>
            {
                bool isLocked = index >= limit;
                return BoardMapper.MapToDto(member.Board, isLocked, member.IsStarred);
            })
            .ToList();

        return boardDtos;
    }
}