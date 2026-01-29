using Microsoft.AspNetCore.Components.Forms;

namespace TaskTrackerApp.Frontend.Domain.DTOs.CardComments;

public class CreateCardCommentDto
{
    public int CardId { get; set; }

    public string? Text { get; set; }

    public int CreatedById { get; set; }

    public IReadOnlyList<IBrowserFile> Files { get; set; } = new List<IBrowserFile>();
}