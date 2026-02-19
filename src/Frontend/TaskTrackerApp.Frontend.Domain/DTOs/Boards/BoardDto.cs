namespace TaskTrackerApp.Frontend.Domain.DTOs.Boards;

public class BoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime LastModified { get; set; }

    public bool IsLocked { get; set; }

    public int CreatedById { get; set; }
}