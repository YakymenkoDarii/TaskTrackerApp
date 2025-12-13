namespace TaskTrackerApp.Domain.DTOs.Card;

public class CreateCardDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int ColumnId { get; set; }
    public int BoardId { get; set; }
    public int? AssigneeId { get; set; }
    public int CreatedById { get; set; }
}