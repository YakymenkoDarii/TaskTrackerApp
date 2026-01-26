using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Events.Card;
public record CardCreatedEvent(
    int CardId,
    int BoardId,
    int ColumnId,
    string Title,
    string? Description,
    int? AssigneeId,
    DateTime? DueDate,
    int Position,
    CardPriority Priority
);