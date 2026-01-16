using MediatR;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Errors;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;

public class RespondToInvitationCommandHandler : IRequestHandler<RespondToInvitationCommand, Result>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public RespondToInvitationCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result> Handle(RespondToInvitationCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        var invitation = await uow.BoardInvitationsRepository.GetById(request.InvitationId);
        if (invitation == null) return Result.Failure(new Error("Invitation.NotFound", "Invitation not found."));

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

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}