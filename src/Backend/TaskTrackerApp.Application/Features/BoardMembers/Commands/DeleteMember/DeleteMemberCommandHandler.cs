using MediatR;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors.Auth;
using TaskTrackerApp.Domain.Errors.Board;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Events.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardMembers.Commands.DeleteMember;

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBoardNotifier _boardNotifier;

    public DeleteMemberCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService,
        IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId is null)
        {
            return Result.Failure(AuthErrors.NotAuthenticated);
        }

        var board = await uow.BoardRepository.GetById(request.BoardId);
        if (board == null)
        {
            return Result.Failure(BoardErrors.NotFound);
        }

        var boardMembers = await uow.BoardMembersRepository.GetByBoardId(request.BoardId);

        var membersList = boardMembers.ToList();

        var currentUserMember = membersList.FirstOrDefault(c => c.UserId == currentUserId);
        var targetMember = membersList.FirstOrDefault(m => m.UserId == request.MemberId);

        if (currentUserMember == null) return Result.Failure(BoardErrors.NotFound);
        if (targetMember == null) return Result.Failure(BoardMemberErrors.NotFound);

        bool isSelfRemoval = (currentUserMember.UserId == targetMember.UserId);

        if (!isSelfRemoval && currentUserMember.Role != BoardRole.Admin)
        {
            return Result.Failure(BoardMemberErrors.NotAuthorized);
        }

        if (targetMember.Role == BoardRole.Admin)
        {
            var adminCount = membersList.Count(m => m.Role == BoardRole.Admin);
            var totalMembers = membersList.Count;

            if (adminCount <= 1)
            {
                if (totalMembers > 1)
                {
                    return Result.Failure(BoardMemberErrors.LastAdminCannotBeDemoted);
                }

                if (totalMembers == 1)
                {
                    await uow.BoardRepository.DeleteAsync(board.Id);

                    await uow.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
            }
        }
        await uow.BoardMembersRepository.DeleteAsync(targetMember.Id);

        await uow.SaveChangesAsync(cancellationToken);

        var evt = new BoardMemberRemovedEvent(board.Id, targetMember.UserId);
        await _boardNotifier.NotifyMemberRemovedAsync(evt);

        return Result.Success();
    }
}