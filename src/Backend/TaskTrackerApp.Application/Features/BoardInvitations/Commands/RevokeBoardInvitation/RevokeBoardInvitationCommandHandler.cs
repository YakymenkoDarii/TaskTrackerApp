using MediatR;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RevokeBoardInvitation;

public class RevokeBoardInvitationCommandHandler : IRequestHandler<RevokeBoardInvitationCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IInvitationNotifier _notifier;

    public RevokeBoardInvitationCommandHandler(IUnitOfWorkFactory uowFactory, IInvitationNotifier notifier)
    {
        _uowFactory = uowFactory;
        _notifier = notifier;
    }

    public async Task<Result> Handle(RevokeBoardInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitation = await uow.BoardInvitationsRepository.GetById(request.InvitationId);

        if (invitation == null)
        {
            return Result.Failure((new Error("Invitation.NotFound", "Invitation not found.")));
        }

        var inviteeId = invitation.InviteeId;

        uow.BoardInvitationsRepository.DeleteAsync(invitation.Id);

        await uow.SaveChangesAsync(cancellationToken);

        await _notifier.NotifyUserInviteRevokedAsync(inviteeId.Value, request.InvitationId);

        return Result.Success();
    }
}