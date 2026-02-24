using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Boards.Requests;

public class UpdateStarRequest
{
    public bool IsStarred { get; set; }
}