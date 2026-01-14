namespace TaskTrackerApp.Domain.Errors.Board;

public static class BoardErrors
{
    public static readonly Error NotFound = new(
        "Board.NotFound", "Board not found.");
}