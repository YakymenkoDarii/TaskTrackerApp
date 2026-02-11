using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Meeting;

public class ScreenShareSession
{
    public string PeerId { get; set; }

    public string StreamId { get; set; }

    public string DisplayName { get; set; }
}