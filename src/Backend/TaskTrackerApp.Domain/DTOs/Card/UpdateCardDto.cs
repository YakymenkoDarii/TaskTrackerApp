using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.Card;

public class UpdateCardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public int UpdatedById { get; set; }

    public int Position { get; set; }

    public bool IsCompleted { get; set; }

    public CardPriority Priority { get; set; }
}