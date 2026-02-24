using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Application.Mappers.BoardMappers;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Errors.User;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetSharedWithMeBoards;

internal class GetSharedWithMeBoardsQueryHandler : IRequestHandler<GetSharedWithMeBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public GetSharedWithMeBoardsQueryHandler(
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetSharedWithMeBoardsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return UserErrors.Unauthorized;
        }

        using var uow = _uowFactory.Create();

        var memberships = await uow.BoardMembersRepository.GetMembershipsWithBoardDetailsAsync(userId.Value);

        var boardDtos = new List<BoardDto>();

        var membershipsByOwner = memberships
            .Where(m => m.Board.CreatedById != userId.Value)
            .GroupBy(m => m.Board.CreatedById);

        foreach (var ownerGroup in membershipsByOwner)
        {
            var ownerId = ownerGroup.Key;
            var owner = await uow.UserRepository.GetById(ownerId);

            if (owner.IsPro)
            {
                foreach (var member in ownerGroup)
                {
                    boardDtos.Add(BoardMapper.MapToDto(member.Board, false, member.IsStarred, member.ThemeColor));
                }
            }
            else
            {
                var allOwnersBoards = await uow.BoardRepository.GetByCreatorIdAsync(ownerId);

                var unlockedBoardIds = allOwnersBoards
                    .OrderByDescending(b => b.LastModified)
                    .Take(3)
                    .Select(b => b.Id)
                    .ToHashSet();

                foreach (var member in ownerGroup)
                {
                    bool isLocked = !unlockedBoardIds.Contains(member.Board.Id);
                    boardDtos.Add(BoardMapper.MapToDto(member.Board, isLocked, member.IsStarred, member.ThemeColor));
                }
            }
        }

        return boardDtos;
    }
}