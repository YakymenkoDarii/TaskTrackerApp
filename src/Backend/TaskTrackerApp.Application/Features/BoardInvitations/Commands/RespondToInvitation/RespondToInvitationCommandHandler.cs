using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Events.BoardMember;
using TaskTrackerApp.Domain.Events.Invitations;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;

public class RespondToInvitationCommandHandler : IRequestHandler<RespondToInvitationCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IInvitationNotifier _notifier;
    private readonly IBoardNotifier _boardNotifier;

    public RespondToInvitationCommandHandler(
        IUnitOfWorkFactory uowFactory,
        IInvitationNotifier notifier,
        IBoardNotifier boardNotifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
        _boardNotifier = boardNotifier;
    }

    public async Task<Result> Handle(RespondToInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitation = await uow.BoardInvitationsRepository.GetById(request.InvitationId);
        if (invitation == null)
            return Result.Failure(new Error("Invitation.NotFound", "Invitation not found."));

        var board = await uow.BoardRepository.GetById(invitation.BoardId);

        if (invitation.Status != InvitationStatus.Pending)
            return Result.Failure(new Error("Invitation.Processed", $"Invitation is already {invitation.Status}."));

        string inviteeName = invitation.InviteeEmail;
        string? inviteeAvatar = null;
        int? inviteeId = invitation.InviteeId;

        if (inviteeId.HasValue)
        {
            var invitee = await uow.UserRepository.GetById(inviteeId.Value);
            if (invitee != null)
            {
                inviteeName = invitee.DisplayName ?? invitee.Email;
                inviteeAvatar = invitee.AvatarUrl;
            }
        }

        if (request.IsAccepted)
        {
            invitation.Status = InvitationStatus.Accepted;

            if (inviteeId.HasValue)
            {
                var newMember = new BoardMember
                {
                    BoardId = invitation.BoardId,
                    UserId = inviteeId.Value,
                    Role = invitation.Role
                };
                await uow.BoardMembersRepository.AddAsync(newMember);

                var evt = new BoardMemberAddedEvent(
                    board.Id,
                    inviteeId.Value,
                    inviteeName,
                    invitation.Role.ToString(),
                    inviteeAvatar
                );

                await _boardNotifier.NotifyMemberAddedAsync(evt);
            }
        }
        else
        {
            invitation.Status = InvitationStatus.Rejected;

            var revokedEvt = new BoardInvitationRevokedEvent(board.Id, invitation.Id);
            await _boardNotifier.NotifyInvitationRevokedAsync(revokedEvt);
        }

        await uow.SaveChangesAsync(cancellationToken);

        await _notifier.NotifySenderInviteRespondedAsync(
            invitation.SenderId,
            inviteeName,
            board.Title,
            request.IsAccepted
        );

        return Result.Success();
    }
}