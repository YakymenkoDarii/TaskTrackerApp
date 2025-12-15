namespace TaskTrackerApp.Domain.DTOs.Column;

public class CreateColumnDto
{
    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int BoardId { get; set; }
}