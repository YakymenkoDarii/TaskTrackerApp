namespace TaskTrackerApp.Domain.Events.Card;
public record CardMovedEvent(
    int CardId,
    int BoardId,
    int OldColumnId,
    int NewColumnId,
    int NewPosition
);