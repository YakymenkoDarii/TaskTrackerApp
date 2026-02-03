using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Functions.Functions.Data.Dtos.BoardMember;

public class BoardMemberDto
{
    public int UserId { get; set; }

    public string Role { get; set; } = string.Empty;
}