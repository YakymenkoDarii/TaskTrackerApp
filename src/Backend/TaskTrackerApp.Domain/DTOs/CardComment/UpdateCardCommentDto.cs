using Microsoft.AspNetCore.Http;

namespace TaskTrackerApp.Domain.DTOs.CardComment;

public class UpdateCardCommentDto
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public List<IFormFile>? NewAttachments { get; set; }

    public List<int>? KeepAttachmentIds { get; set; }
}