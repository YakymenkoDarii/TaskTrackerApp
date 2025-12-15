using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities;

public class Board : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public User CreatedBy { get; set; }
    public User UpdatedBy { get; set; }

    public IList<Column> Columns { get; set; } = new List<Column>();
    public IList<Card> Cards { get; set; } = new List<Card>();
    public IList<BoardMember> Members { get; set; } = new List<BoardMember>();
}