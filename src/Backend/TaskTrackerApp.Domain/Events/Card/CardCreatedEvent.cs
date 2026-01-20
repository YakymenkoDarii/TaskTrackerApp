namespace TaskTrackerApp.Domain.Events.Card;
public record CardCreatedEvent(
    int Id,
    int BoardId,
    int ColumnId,
    string Title,
    string? Description,
    int? AssigneeId,
    string? Priority
);