namespace TaskTrackerApp.Domain.Events.Labels;
public record LabelDeletedEvent(
    int BoardId,
    int LabelId
);