namespace TaskTrackerApp.Domain.Entities;

public class ArchivedBoard
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int OriginalBoardId { get; set; }
}