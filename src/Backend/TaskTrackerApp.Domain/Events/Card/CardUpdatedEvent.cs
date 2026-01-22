using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Events.Card;
public record CardUpdatedEvent(
    int CardId,
    int BoardId,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    CardPriority Priority,
    int? AssigneeId
);