using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RevokeBoardInvitation;

public class RevokeBoardInvitationCommandHandler : IRequestHandler<RevokeBoardInvitationCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public RevokeBoardInvitationCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(RevokeBoardInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitation = await uow.BoardInvitationsRepository.GetById(request.InvitationId);

        if (invitation == null)
        {
            return Result.Failure((new Error("Invitation.NotFound", "Invitation not found.")));
        }

        uow.BoardInvitationsRepository.DeleteAsync(invitation.Id);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}