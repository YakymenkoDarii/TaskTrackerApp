using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Boards;

public class CreateBoardDto
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public BoardThemeColor ThemeColor { get; set; } = BoardThemeColor.DefaultBlue;
}