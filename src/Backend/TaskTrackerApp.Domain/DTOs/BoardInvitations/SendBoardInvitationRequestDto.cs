using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.BoardInvitations;

public class SendBoardInvitationRequestDto
{
    public int BoardId { get; set; }

    public string InviteeEmail { get; set; }

    public BoardRole Role { get; set; }
}