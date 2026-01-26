namespace TaskTrackerApp.Frontend.Domain.Events.Column;
public record ColumnMovedEvent(
    int ColumnId,
    int BoardId,
    int NewPosition
);