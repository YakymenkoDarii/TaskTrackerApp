namespace TaskTrackerApp.Domain.Errors.Board;

public static class BoardErrors
{
    public static readonly Error NotFound = new(
        "Board.NotFound", "Board not found.");

    public static readonly Error Archived = new(
        "Board.Archived", "This board is archived and cannot be modified.");
}