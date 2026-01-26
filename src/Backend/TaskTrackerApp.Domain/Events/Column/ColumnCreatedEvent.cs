namespace TaskTrackerApp.Domain.Events.Column;
public record ColumnCreatedEvent(
    int Id,
    int BoardId,
    string Title,
    int Position
);