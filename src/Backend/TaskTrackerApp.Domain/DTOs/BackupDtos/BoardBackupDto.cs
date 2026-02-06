using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class BoardBackupDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsArchived { get; set; }

    public int CreatedById { get; set; }

    public List<MemberBackupDto> Members { get; set; } = new();

    public List<ColumnBackupDto> Columns { get; set; } = new();

    public List<LabelBackupDto> Labels { get; set; } = new();
}