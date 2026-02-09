namespace TaskTrackerApp.Domain.DTOs.Board;

public class ArchivedBoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public bool CanUnarchive { get; set; }
}