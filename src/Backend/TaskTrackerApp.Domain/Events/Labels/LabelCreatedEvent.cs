namespace TaskTrackerApp.Domain.Events.Labels;
public record LabelCreatedEvent(
    int BoardId,
    int LabelId,
    string Name,
    string Color
);