using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.Boards.Commands.TransferOwnership;

internal class TransferOwnershipCommandHandler : IRequestHandler<TransferOwnershipCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public TransferOwnershipCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(TransferOwnershipCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var board = await uow.BoardRepository.GetById(request.BoardId);

        if (board == null)
        {
            return Result.Failure(new Error("NotFound", "Board not found"));
        }

        var userId = _currentUserService.UserId;

        if (userId != board.CreatedById)
        {
            return Result.Failure(new Error("Unauthorized", "Only the owner can transfer this board"));
        }

        var isNewOwnerMember = await uow.BoardMembersRepository.ExistsAsync(request.BoardId, request.TransferUserId);
        if (!isNewOwnerMember)
        {
            return Result.Failure(new Error("InvalidTransfer", "The new owner must be a member of the board first."));
        }

        board.CreatedById = request.TransferUserId;

        board.LastModified = DateTime.UtcNow;

        await uow.BoardRepository.UpdateAsync(board);
        await uow.SaveChangesAsync();

        return Result.Success();
    }
}