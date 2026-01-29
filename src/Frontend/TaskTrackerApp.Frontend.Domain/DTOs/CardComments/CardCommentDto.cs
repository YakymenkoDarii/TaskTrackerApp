using TaskTrackerApp.Frontend.Domain.DTOs.CommentAttachments;

namespace TaskTrackerApp.Frontend.Domain.DTOs.CardComments;

public class CardCommentDto
{
    public int Id { get; set; }

    public string? Text { get; set; } = string.Empty;

    public bool IsEdited { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CreatedById { get; set; }

    public string AuthorName { get; set; }

    public string? AuthorAvatarUrl { get; set; }

    public List<CommentAttachmentDto> Attachments { get; set; } = new();
}