using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain;

public class RecentBoardItem
{
    public int BoardId { get; set; }

    public DateTime LastViewed { get; set; }
}