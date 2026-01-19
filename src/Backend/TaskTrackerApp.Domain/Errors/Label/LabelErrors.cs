namespace TaskTrackerApp.Domain.Errors.Label;

public class LabelErrors
{
    public static readonly Error NotFound = new(
    "Label.NotFound", "Label not found.");
}