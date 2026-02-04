using TaskTrackerApp.Functions.Functions.Data.Enums;

namespace TaskTrackerApp.Functions.Functions.Data.Entities;

public class Card
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int Position { get; set; }

    public CardPriority Priority { get; set; } = CardPriority.Low;

    public int ColumnId { get; set; }

    public int BoardId { get; set; }

    public int? AssigneeId { get; set; }

    public Column Column { get; set; }

    public Board Board { get; set; }

    public User? AssigneeUser { get; set; }

    public ICollection<CardComment> Comments { get; set; } = new List<CardComment>();

    public ICollection<Label> Labels { get; set; } = new List<Label>();
}