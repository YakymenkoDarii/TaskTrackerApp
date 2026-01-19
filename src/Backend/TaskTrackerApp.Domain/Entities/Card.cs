using TaskTrackerApp.Domain.Entities.Base;
using TaskTrackerApp.Domain.Enums;

namespace TaskTrackerApp.Domain.Entities;

public class Card : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public bool IsArchived { get; set; } = false;

    public bool IsCompleted { get; set; } = false;

    public int Position { get; set; }

    public CardPriority Priority { get; set; } = CardPriority.Low;

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public Column Column { get; set; }

    public Board Board { get; set; }

    public User? AssigneeUser { get; set; }

    public User CreatedBy { get; set; }

    public User UpdatedBy { get; set; }

    public ICollection<CardComment> Comments { get; set; } = new List<CardComment>();

    public ICollection<Label> Labels { get; set; } = new List<Label>();
}