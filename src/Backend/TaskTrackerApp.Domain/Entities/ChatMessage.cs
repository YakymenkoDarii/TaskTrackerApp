namespace TaskTrackerApp.Domain.Entities;

public class ChatMessage
{
    public string Role { get; set; }

    public string Content { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}