namespace TaskTrackerApp.Frontend.Domain.Events.Card;
public record CardMovedEvent(
    int CardId,
    int NewColumnId,
    int NewPosition
);