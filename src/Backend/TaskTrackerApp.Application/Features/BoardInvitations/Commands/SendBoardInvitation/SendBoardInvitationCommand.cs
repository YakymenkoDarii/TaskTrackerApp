using MediatR;
using TaskTrackerApp.Domain.Enums;
using TaskTrackerApp.Domain.Results;

namespace TaskTrackerApp.Application.Features.BoardInvitations.Commands.SendBoardInvitation;

public class SendBoardInvitationCommand : IRequest<Result<int>>
{
    public int BoardId { get; set; }

    public string InviteeEmail { get; set; }

    public BoardRole Role { get; set; }

    public int SenderId { get; set; }
}