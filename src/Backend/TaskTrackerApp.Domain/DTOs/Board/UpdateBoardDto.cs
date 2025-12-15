namespace TaskTrackerApp.Domain.DTOs.Board;

public class UpdateBoardDto
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedById { get; set; }
}