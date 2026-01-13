using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;

public class BoardMemberDto
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }

    public string AvatarUrl { get; set; }
}