namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class ColumnBackupDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Position { get; set; }

    public List<CardBackupDto> Cards { get; set; } = new();
}