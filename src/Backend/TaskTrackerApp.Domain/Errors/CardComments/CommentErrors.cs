namespace TaskTrackerApp.Domain.Errors.CardComments;

public static class CommentErrors
{
    public static readonly Error NotFound = new(
        "Comment.NotFound", "Comment not found");

    public static readonly Error NotOwner = new(
        "Comment.NotOwner", "You can only delete your own comments");

    public static readonly Error Empty = new(
        "Comment.Empty", "Comment cannot be completely empty.");
}