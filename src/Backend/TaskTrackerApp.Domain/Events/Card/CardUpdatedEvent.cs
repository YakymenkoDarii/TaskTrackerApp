namespace TaskTrackerApp.Domain.Events.Card;
public record CardUpdatedEvent(
    int Id,
    int BoardId,
    int ColumnId,
    string Title,
    int? AssigneeId,
    bool IsCompleted
);