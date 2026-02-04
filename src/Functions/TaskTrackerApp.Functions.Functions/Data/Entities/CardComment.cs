namespace TaskTrackerApp.Functions.Functions.Data.Entities;

public class CardComment
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CreatedById { get; set; }

    public string? Text { get; set; } = string.Empty;

    public bool IsEdited { get; set; }

    public int CardId { get; set; }

    public Card Card { get; set; }

    public User CreatedBy { get; set; }

    public ICollection<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();
}