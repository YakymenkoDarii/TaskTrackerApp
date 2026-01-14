using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;

public class SendBoardInvitationRequestDto
{
    public int BoardId { get; set; }

    public string InviteeEmail { get; set; }

    public BoardRole Role { get; set; }
}