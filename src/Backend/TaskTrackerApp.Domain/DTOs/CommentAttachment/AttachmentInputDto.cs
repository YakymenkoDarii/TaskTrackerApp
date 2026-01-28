namespace TaskTrackerApp.Domain.DTOs.CommentAttachment;

public class AttachmentInputDto
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public long Size { get; set; }

    public Stream? FileContent { get; set; }

    public string? StoredFileName { get; set; }

    public string? Url { get; set; }
}