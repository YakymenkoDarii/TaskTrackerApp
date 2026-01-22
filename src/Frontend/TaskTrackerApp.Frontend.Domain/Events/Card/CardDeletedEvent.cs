namespace TaskTrackerApp.Frontend.Domain.Events.Card;
public record CardDeletedEvent(
    int Id,
    int BoardId
);