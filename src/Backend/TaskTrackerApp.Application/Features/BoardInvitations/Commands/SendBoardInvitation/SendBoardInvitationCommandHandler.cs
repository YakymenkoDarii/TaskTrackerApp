using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Errors.BoardMember;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.SendBoardInvitation;

public class SendBoardInvitationCommandHandler : IRequestHandler<SendBoardInvitationCommand, Result<int>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IInvitationNotifier _notifier;

    public SendBoardInvitationCommandHandler(IUnitOfWorkFactory uowFactory, IInvitationNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result<int>> Handle(SendBoardInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var boardMembers = await uow.BoardMembersRepository.GetByBoardId(request.BoardId);
        var senderMember = boardMembers.FirstOrDefault(m => m.UserId == request.SenderId);

        if (senderMember == null || senderMember.Role != BoardRole.Admin)
        {
            return Result<int>.Failure(BoardMemberErrors.NotAuthorized);
        }

        var existingUser = await uow.UserRepository.GetByEmailAsync(request.InviteeEmail);
        if (existingUser != null)
        {
            var isAlreadyMember = await uow.BoardMembersRepository
                .ExistsAsync(request.BoardId, existingUser.Id);

            if (isAlreadyMember)
            {
                return Result<int>.Failure(new Error("Invitation.UserIsMember", "User is already a member of this board."));
            }
        }

        var existingInvite = await uow.BoardInvitationsRepository.GetPendingByEmailAsync(request.BoardId, request.InviteeEmail);
        if (existingInvite != null)
        {
            return Result<int>.Failure(new Error("Invitation.AlreadyPending", "An invitation is already pending for this user."));
        }

        var invitation = new BoardInvitation
        {
            BoardId = request.BoardId,
            SenderId = request.SenderId,
            InviteeEmail = request.InviteeEmail,
            InviteeId = existingUser?.Id,
            Role = request.Role,
            Status = InvitationStatus.Pending,
        };

        await uow.BoardInvitationsRepository.AddAsync(invitation);

        var board = await uow.BoardRepository.GetById(request.BoardId);
        var senderUser = await uow.UserRepository.GetById(request.SenderId);

        await uow.SaveChangesAsync(cancellationToken);

        if (existingUser != null)
        {
            await _notifier.NotifyUserReceivedInviteAsync(
                existingUser.Id,
                invitation.Id,
                request.BoardId,
                senderUser.DisplayName ?? senderUser.Email,
                board.Title
            );
        }

        return Result<int>.Success(invitation.Id);
    }
}