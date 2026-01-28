using TaskTrackerApp.Domain.DTOs.CommentAttachment;

namespace TaskTrackerApp.Domain.Events.Comment;
public record CommentUpdatedEvent(
    int Id,
    int CardId,
    string Text,
    DateTime UpdatedAt,
    List<CommentAttachmentDto> Attachments
);