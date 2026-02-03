using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.ArchiveBoard;

public class ChangeArchiveStatusBoardCommandHandler : IRequestHandler<ChangeArchiveStatusBoardCommand, Result>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICosmosJobTracker _cosmosJobTracker;

    public ChangeArchiveStatusBoardCommandHandler(
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory,
        ICosmosJobTracker cosmosJobTracker)
    {
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
        _cosmosJobTracker = cosmosJobTracker;
    }

    public async Task<Result> Handle(ChangeArchiveStatusBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        if (_currentUserService.UserId is not { } userId)
        {
            return BoardMemberErrors.NotAuthorized;
        }

        var boardMember = await uow.BoardMembersRepository
            .GetMemberAsync(request.BoardId, userId);

        if (boardMember == null)
        {
            return BoardMemberErrors.NotFound;
        }

        if (boardMember.Role != BoardRole.Admin)
        {
            return BoardMemberErrors.NotAuthorized;
        }

        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board == null)
        {
            return BoardErrors.NotFound;
        }

        board.IsArchived = !board.IsArchived;

        board.IsBackedUp = false;

        if (board.IsArchived == false)
        {
            await _cosmosJobTracker.DeleteJobByBoardIdAsync(request.BoardId);
        }

        uow.BoardRepository.UpdateAsync(board);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}