using TaskTrackerApp.Domain.DTOs.CardComment;

namespace TaskTrackerApp.Domain.DTOs.Card;

public class CardExportDto : CardDto
{
    public new List<CardCommentExportDto> Comments { get; set; } = new();
}