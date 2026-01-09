namespace TaskTrackerApp.Frontend.Domain.DTOs.Boards;

public class UpdateBoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int UpdatedById { get; set; }
}