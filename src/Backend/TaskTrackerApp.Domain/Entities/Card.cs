using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities;

public class Card : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }

    public int ColumnId { get; set; }
    public int BoardId { get; set; }
    public int? AssigneeId { get; set; }

    public Column Column { get; set; }
    public Board Board { get; set; }
    public User? AssigneeUser { get; set; }

    public User CreatedBy { get; set; }
    public User UpdatedBy { get; set; }
}