namespace TaskTrackerApp.Domain.Entities;

public class CommentAttachment
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string StoredFileName { get; set; }

    public string ContentType { get; set; }

    public string Url { get; set; }

    public long Size { get; set; }

    public int CommentId { get; set; }

    public CardComment Comment { get; set; }
}