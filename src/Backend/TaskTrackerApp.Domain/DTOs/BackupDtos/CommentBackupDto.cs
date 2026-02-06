using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class CommentBackupDto
{
    public string Text { get; set; } = string.Empty;

    public int CreatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<AttachmentBackupDto> Attachments { get; set; } = new();
}