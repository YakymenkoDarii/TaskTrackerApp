using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.DTOs.BackupDtos;

public class MemberBackupDto
{
    public int UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoardRole Role { get; set; }
}