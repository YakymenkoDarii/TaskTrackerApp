using TaskTrackerApp.Domain.DTOs.CommentAttachment;

namespace TaskTrackerApp.Domain.DTOs.CardComment;

public class CardCommentExportDto : CardCommentDto
{
    public new List<AttachmentExportDto> Attachments { get; set; } = new();
}