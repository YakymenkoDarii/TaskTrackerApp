namespace TaskTrackerApp.Domain.Entities;

public class BoardMember
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BoardId { get; set; }

    public string? Role { get; set; }

    public DateTime? JoinedAt { get; set; } = DateTime.UtcNow;

}
