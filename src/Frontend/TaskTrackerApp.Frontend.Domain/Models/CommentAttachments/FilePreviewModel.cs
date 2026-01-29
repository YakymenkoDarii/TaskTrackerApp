using Microsoft.AspNetCore.Components.Forms;

namespace TaskTrackerApp.Frontend.Domain.Models.CommentAttachments;

public class FilePreviewModel
{
    public IBrowserFile File { get; set; }

    public string? Url { get; set; }

    public bool IsImage { get; set; }
}