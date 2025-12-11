namespace TaskTrackerApp.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Tag { get; set; }

    public string DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    //Foreign keys
    public IList<Card> AssignedTasks { get; set; }

    public IList<Board> CreatedBoards { get; set; }
}