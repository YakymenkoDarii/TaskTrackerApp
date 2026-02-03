namespace TaskTrackerApp.Domain.DTOs.CommentAttachment;

public class AttachmentExportDto
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public long Size { get; set; }

    public string Url { get; set; }
}