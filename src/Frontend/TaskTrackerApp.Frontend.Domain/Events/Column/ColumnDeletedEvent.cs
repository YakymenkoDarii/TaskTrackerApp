namespace TaskTrackerApp.Frontend.Domain.Events.Column;
public record ColumnDeletedEvent(
    int ColumnId,
    int BoardId
);