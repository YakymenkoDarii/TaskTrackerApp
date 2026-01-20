namespace TaskTrackerApp.Domain.Events.Column;
public record ColumnDeletedEvent(
    int ColumnId,
    int BoardId
);