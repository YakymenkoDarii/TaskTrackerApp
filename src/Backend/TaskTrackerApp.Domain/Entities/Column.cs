using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities;

public class Column : BaseEntity
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int BoardId { get; set; }

    // Foreign Keys
    public IList<Card> Cards { get; set; }

    public Board Board { get; set; }
}