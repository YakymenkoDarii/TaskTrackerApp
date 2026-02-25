using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Entities;

public class BoardMember
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BoardId { get; set; }

    public BoardRole Role { get; set; } = BoardRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public bool IsStarred { get; set; } = false;

    public BoardThemeColor ThemeColor { get; set; } = BoardThemeColor.DefaultBlue;

    public User User { get; set; }

    public Board Board { get; set; }
}