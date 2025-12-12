namespace TaskTrackerApp.Domain.DTOs.Board;

public class UpdateBoardDto
{
    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedBy { get; set; }
}
