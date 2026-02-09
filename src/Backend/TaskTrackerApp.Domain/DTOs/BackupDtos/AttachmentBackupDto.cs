namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class AttachmentBackupDto
{
    public string FileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}