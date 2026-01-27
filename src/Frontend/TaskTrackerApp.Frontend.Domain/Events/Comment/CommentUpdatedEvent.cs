using TaskTrackerApp.Frontend.Domain.DTOs.CommentAttachments;

namespace TaskTrackerApp.Frontend.Domain.Events.Comment;
public record CommentUpdatedEvent(
    int Id,
    int CardId,
    string Text,
    DateTime UpdatedAt,
    List<CommentAttachmentDto> Attachments
);