using TaskTrackerApp.Domain.Enums;

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

    public Role Role { get; set; } = Role.User;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }

    //Foreign keys
    public IList<Card> AssignedTasks { get; set; } = new List<Card>();

    public IList<Board> CreatedBoards { get; set; } = new List<Board>();

    public IList<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();
}