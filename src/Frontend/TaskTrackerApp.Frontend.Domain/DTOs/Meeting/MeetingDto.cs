namespace TaskTrackerApp.Frontend.Domain.DTOs.Meeting;

public class MeetingDto
{
    public int BoardId { get; set; }

    public DateTime StartTime { get; set; }

    public List<MeetingParticipant> Participants { get; set; } = new();
}