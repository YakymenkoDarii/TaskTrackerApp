namespace TaskTrackerApp.Functions.Functions.Data.Entities;

public class Column
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public int BoardId { get; set; }

    public Board Board { get; set; }

    public IList<Card> Cards { get; set; } = new List<Card>();
}