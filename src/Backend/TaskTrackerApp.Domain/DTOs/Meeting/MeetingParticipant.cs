namespace TaskTrackerApp.Domain.DTOs.Meeting;

public class MeetingParticipant
{
    public string PeerId { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public bool IsMuted { get; set; } = true;

    public bool IsVideoOff { get; set; } = true;
}