namespace TaskTrackerApp.Domain.DTOs.BoardInvitations;

public class BoardInvitationDto
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public int? InviteeId { get; set; }

    public string InviteeEmail { get; set; }

    public string InviteeAvatarUrl { get; set; }

    public string Role { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}