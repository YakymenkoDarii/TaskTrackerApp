using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities;

public class Board : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }

    // Foreign Keys
    public IList<Column> Columns { get; set; }
}