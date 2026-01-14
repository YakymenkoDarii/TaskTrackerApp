using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RespondToInvitation;

public class RespondToInvitationCommand : IRequest<Result>
{
    public int InvitationId { get; set; }

    public bool IsAccepted { get; set; }
}