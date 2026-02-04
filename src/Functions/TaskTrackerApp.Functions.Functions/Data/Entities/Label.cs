namespace TaskTrackerApp.Functions.Functions.Data.Entities;

public class Label
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#FFFFFF";

    public int BoardId { get; set; }

    public Board Board { get; set; }

    public ICollection<Card> Cards { get; set; } = new List<Card>();
}