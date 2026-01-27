namespace TaskTrackerApp.Domain.DTOs.CommentAttachment;

public class CommentAttachmentDto
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string Url { get; set; }

    public string ContentType { get; set; }

    public long Size { get; set; }

    public bool IsImage => ContentType?.StartsWith("image/") == true;
}