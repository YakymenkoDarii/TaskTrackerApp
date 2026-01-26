namespace TaskTrackerApp.Domain.Events.Card;
public record CardMovedEvent(
    int CardId,
    int BoardId,
    int NewColumnId,
    int NewPosition
);