using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.BoardMembers
{
    public class BoardMembersDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BoardId { get; set; }

        public string? Role { get; set; }

        public DateTime? JoinedAt { get; set; }
    }
}
