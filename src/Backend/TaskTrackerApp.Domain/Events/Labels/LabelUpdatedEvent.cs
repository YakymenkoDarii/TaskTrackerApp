namespace TaskTrackerApp.Domain.Events.Labels;
public record LabelUpdatedEvent(
    int BoardId,
    int LabelId,
    string Name,
    string Color
);