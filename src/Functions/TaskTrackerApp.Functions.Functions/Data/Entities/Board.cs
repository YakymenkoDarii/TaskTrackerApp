namespace TaskTrackerApp.Functions.Functions.Data.Entities;

public class Board
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public ICollection<Column> Columns { get; set; } = new List<Column>();

    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}