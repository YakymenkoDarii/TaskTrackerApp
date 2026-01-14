using MediatR;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.RevokeBoardInvitation;

public class RevokeBoardInvitationCommand : IRequest<Result>
{
    public int InvitationId { get; set; }
}