using TaskTrackerApp.Functions.Functions.Data.Dtos.Attachment;

namespace TaskTrackerApp.Functions.Functions.Data.Dtos.Comment;

public class CommentDto
{
    public string Text { get; set; } = string.Empty;

    public int CreatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<AttachmentDto> Attachments { get; set; } = new();
}