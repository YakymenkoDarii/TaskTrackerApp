using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.DTOs.Board;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Queries.GetAllBoards;

public class GetAllBoardsQueryHandler : IRequestHandler<GetAllBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ICurrentUserService _currentUserService;

    public GetAllBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, ICurrentUserService currentUserService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        var memberships = await uow.BoardMembersRepository.GetByUserIdAsync(request.UserId);
        var currentUser = await uow.UserRepository.GetById(request.UserId);

        var boardDtos = new List<BoardDto>();

        var allBoards = memberships.Select(m => m.Board).ToList();

        var ownedBoards = allBoards.Where(b => b.CreatedById == request.UserId).ToList();
        var guestBoards = allBoards.Where(b => b.CreatedById != request.UserId).ToList();

        int limit = currentUser.IsPro ? int.MaxValue : 3;

        var sortedOwnedBoards = ownedBoards
            .OrderByDescending(b => b.LastModified)
            .ToList();

        for (int i = 0; i < sortedOwnedBoards.Count; i++)
        {
            var board = sortedOwnedBoards[i];

            bool isLocked = i >= limit;

            //boardDtos.Add(BoardMapper.MapToDto(board, isLocked, false));
        }

        foreach (var board in guestBoards)
        {
            //boardDtos.Add(BoardMapper.MapToDto(board, isLocked: false, false));
        }

        return Result<IEnumerable<BoardDto>>.Success(
            boardDtos.OrderByDescending(b => b.LastModified)
        );
    }
}