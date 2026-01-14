namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;

public class MyInvitationDto
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public string BoardTitle { get; set; }

    public string SenderName { get; set; }

    public string SenderAvatarUrl { get; set; }

    public DateTime SentAt { get; set; }
}