using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;

public class RespondToInvitationCommandHandler : IRequestHandler<RespondToInvitationCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IInvitationNotifier _notifier;

    public RespondToInvitationCommandHandler(IUnitOfWorkFactory uowFactory, IInvitationNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result> Handle(RespondToInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitation = await uow.BoardInvitationsRepository.GetById(request.InvitationId);
        if (invitation == null)
            return Result.Failure(new Error("Invitation.NotFound", "Invitation not found."));

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Failure(new Error("Invitation.Processed", $"Invitation is already {invitation.Status}."));
        }

        if (request.IsAccepted)
        {
            invitation.Status = InvitationStatus.Accepted;

            if (invitation.InviteeId.HasValue)
            {
                var newMember = new BoardMember
                {
                    BoardId = invitation.BoardId,
                    UserId = invitation.InviteeId.Value,
                    Role = invitation.Role
                };
                await uow.BoardMembersRepository.AddAsync(newMember);
            }
        }
        else
        {
            invitation.Status = InvitationStatus.Rejected;
        }

        var board = await uow.BoardRepository.GetById(invitation.BoardId);

        string inviteeName = invitation.InviteeEmail;
        if (invitation.InviteeId.HasValue)
        {
            var invitee = await uow.UserRepository.GetById(invitation.InviteeId.Value);
            if (invitee != null) inviteeName = invitee.DisplayName ?? invitee.Email;
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