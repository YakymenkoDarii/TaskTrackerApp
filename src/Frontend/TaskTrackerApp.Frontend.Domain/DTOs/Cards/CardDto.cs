using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Cards;

public class CardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? AssigneeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsCompleted { get; set; }

    public int ColumnId { get; set; }

    public int Position { get; set; }

    public int BoardId { get; set; }

    public CardPriority Priority { get; set; }
}