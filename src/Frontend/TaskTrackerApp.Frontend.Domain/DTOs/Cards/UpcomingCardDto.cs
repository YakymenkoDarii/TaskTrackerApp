namespace TaskTrackerApp.Frontend.Domain.DTOs.Cards;

public class UpcomingCardDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public int BoardId { get; set; }

    public string BoardTitle { get; set; }

    public string ColumnTitle { get; set; }
}