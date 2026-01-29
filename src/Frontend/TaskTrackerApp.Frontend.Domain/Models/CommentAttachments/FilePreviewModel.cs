using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.Models.CommentAttachments;

public class FilePreviewModel
{
    public IBrowserFile File { get; set; }

    public string? Url { get; set; }

    public bool IsImage { get; set; }
}