using TaskTrackerApp.Functions.Functions.Data.Dtos.Comment;

namespace TaskTrackerApp.Functions.Functions.Data.Dtos.Card;

public class CardDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public bool IsCompleted { get; set; }

    public string Priority { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public int? AssigneeId { get; set; }

    public List<int> LabelIds { get; set; } = new();

    public List<CommentDto> Comments { get; set; } = new();
}