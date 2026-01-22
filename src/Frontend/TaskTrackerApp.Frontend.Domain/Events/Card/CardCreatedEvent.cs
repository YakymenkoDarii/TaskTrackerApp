using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.Events.Card;
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