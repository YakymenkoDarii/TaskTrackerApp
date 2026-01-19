using TaskTrackerApp.Domain.Entities.Base;

namespace TaskTrackerApp.Domain.Entities;

public class CardComment : BaseEntity
{
    public string Text { get; set; } = string.Empty;

    public bool IsEdited { get; set; }

    public int CardId { get; set; }

    public Card Card { get; set; }

    public User CreatedBy { get; set; }
}