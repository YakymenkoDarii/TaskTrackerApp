namespace TaskTrackerApp.Domain.DTOs.Board;
public class CreateBoardDto
{
    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int CreatedBy { get; set; }
}
