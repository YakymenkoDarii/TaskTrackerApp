using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities
{
    public class Card : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }

        // Foreign Keys
        public int ColumnId { get; set; }
        public int BoardId { get; set; }
        public Guid? AssigneeId { get; set; }
    }
}
