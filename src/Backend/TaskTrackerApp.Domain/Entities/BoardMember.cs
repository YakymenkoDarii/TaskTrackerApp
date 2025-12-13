namespace TaskTrackerApp.Domain.Entities;

public class BoardMember
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BoardId { get; set; }

    public string? Role { get; set; }

    public User User { get; set; }
    public Board Board { get; set; }
}