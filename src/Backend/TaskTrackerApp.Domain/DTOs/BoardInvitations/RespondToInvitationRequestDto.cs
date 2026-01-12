using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.BoardInvitations;

public class RespondToInvitationRequestDto
{
    public int InvitationId { get; set; }

    public bool IsAccepted { get; set; }
}