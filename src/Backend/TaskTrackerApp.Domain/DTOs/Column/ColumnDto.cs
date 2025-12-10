using TaskTrackerApp.Domain.DTOs.Card;

namespace TaskTrackerApp.Domain.DTOs.Column;

public class ColumnDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int BoardId { get; set; }
    public List<CardDto> Cards { get; set; } = new List<CardDto>();
}
