using Microsoft.AspNetCore.Components.Forms;

namespace TaskTrackerApp.Frontend.Domain.DTOs.CardComments;

public class UpdateCardCommentDto
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public List<int> KeepAttachmentIds { get; set; } = new();

    public IReadOnlyList<IBrowserFile> NewFiles { get; set; } = new List<IBrowserFile>();
}