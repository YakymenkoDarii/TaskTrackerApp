using TaskTrackerApp.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.Board;

public class BoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime LastModified { get; set; }

    public bool IsLocked { get; set; }

    public int CreatedById { get; set; }

    public bool IsStarred { get; set; }

    public BoardThemeColor ThemeColor { get; set; } = BoardThemeColor.DefaultBlue;

    public List<BoardMemberAvatarDto> Members { get; set; } = new();
}