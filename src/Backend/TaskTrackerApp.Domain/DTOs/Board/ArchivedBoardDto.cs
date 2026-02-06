using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.Board;

public class ArchivedBoardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public bool CanUnarchive { get; set; }
}