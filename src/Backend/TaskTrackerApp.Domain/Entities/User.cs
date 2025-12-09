using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required string Tag { get; set; }

        public required string DisplayName { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        //Foreign keys
        public IList<Card> AssignedTasks { get; set; }

        public IList<Board> CreatedBoards { get; set; }
    }
}
