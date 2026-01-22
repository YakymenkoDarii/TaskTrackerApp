namespace TaskTrackerApp.Frontend.Domain.Events.Labels;
public record LabelDeletedEvent(
    int BoardId,
    int LabelId
);