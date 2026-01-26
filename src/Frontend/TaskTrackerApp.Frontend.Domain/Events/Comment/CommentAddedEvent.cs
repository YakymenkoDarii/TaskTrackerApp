namespace TaskTrackerApp.Frontend.Domain.Events.Comment;
public record CommentAddedEvent(
    int Id,
    int CardId,
    string Text,
    int CreatedById,
    string CreatedByName,
    string? AvatarUrl,
    DateTime CreatedAt
);