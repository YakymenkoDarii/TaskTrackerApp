using TaskTrackerApp.Domain.DTOs.Column;

namespace TaskTrackerApp.Domain.DTOs.Board;

public class BoardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }

    public List<ColumnDto> Columns { get; set; } = new List<ColumnDto>();
}