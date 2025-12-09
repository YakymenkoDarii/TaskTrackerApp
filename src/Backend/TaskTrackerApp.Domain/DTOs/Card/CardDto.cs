namespace TaskTrackerApp.Domain.DTOs.Card;

public class CardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int ColumnId { get; set; }
    public int BoardId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime CreatedAt { get; set; }
}
