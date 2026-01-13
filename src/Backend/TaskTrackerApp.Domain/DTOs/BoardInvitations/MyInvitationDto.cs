using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.BoardInvitations;

public class MyInvitationDto
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public string BoardTitle { get; set; }

    public string SenderName { get; set; }

    public string SenderAvatarUrl { get; set; }

    public DateTime SentAt { get; set; }
}