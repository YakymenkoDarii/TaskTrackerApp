using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Domain.Events.Card;
public record CardUpdatedEvent(
    int CardId,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    CardPriority Priority,
    int? AssigneeId
);