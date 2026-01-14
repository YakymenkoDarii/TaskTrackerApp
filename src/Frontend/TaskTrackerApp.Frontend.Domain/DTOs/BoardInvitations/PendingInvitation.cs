namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;

public class PendingInvitationDto
{
    public int InvitationId { get; set; }

    public int BoardId { get; set; }

    public string BoardTitle { get; set; }

    public string SenderName { get; set; }

    public string Role { get; set; }

    public DateTime SentAt { get; set; }
}