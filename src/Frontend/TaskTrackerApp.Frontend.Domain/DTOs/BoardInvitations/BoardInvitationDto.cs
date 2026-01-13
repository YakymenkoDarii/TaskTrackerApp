using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;

public class BoardInvitationDto
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public int InviteeId { get; set; }

    public string InviteeEmail { get; set; }

    public string InviteeAvatarUrl { get; set; }

    public string Role { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }
}