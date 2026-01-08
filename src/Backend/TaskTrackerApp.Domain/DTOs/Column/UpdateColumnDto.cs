namespace TaskTrackerApp.Domain.DTOs.Column;

public class UpdateColumnDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public int BoardId { get; set; }

    public int UpdatedById { get; set; }

    public int Position { get; set; }
}