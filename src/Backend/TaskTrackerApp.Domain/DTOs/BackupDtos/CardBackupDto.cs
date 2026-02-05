using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class CardBackupDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public bool IsCompleted { get; set; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int? AssigneeId { get; set; }

    public List<int> LabelIds { get; set; } = new();

    public List<CommentBackupDto> Comments { get; set; } = new();
}