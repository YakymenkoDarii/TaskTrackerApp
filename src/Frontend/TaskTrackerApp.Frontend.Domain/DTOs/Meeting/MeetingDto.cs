using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.Meeting;

public class MeetingDto
{
    public int BoardId { get; set; }

    public DateTime StartTime { get; set; }

    public List<string> ParticipantPeerIds { get; set; } = new();
}