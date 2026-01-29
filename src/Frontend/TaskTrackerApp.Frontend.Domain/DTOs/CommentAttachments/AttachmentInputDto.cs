using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.CommentAttachments;

public class AttachmentInputDto
{
    public string FileName { get; set; }

    public string StoredFileName { get; set; }

    public string ContentType { get; set; }

    public string Url { get; set; }

    public long Size { get; set; }
}