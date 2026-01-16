namespace TaskTrackerApp.Frontend.Domain.DTOs.CardComments;

public class CreateCardCommentDto
{
    public int CardId { get; set; }

    public string Text { get; set; }

    public int CreatedById { get; set; }
}