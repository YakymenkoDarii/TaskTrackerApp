using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities
{
    public class Board : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        // Foreign Keys
        public IList<Column> Columns { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
