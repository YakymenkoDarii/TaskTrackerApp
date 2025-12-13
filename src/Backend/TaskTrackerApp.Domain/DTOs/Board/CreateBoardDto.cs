namespace TaskTrackerApp.Domain.DTOs.Board;

public class CreateBoardDto
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int CreatedById { get; set; }
}