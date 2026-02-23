using TaskTrackerApp.Domain.DTOs.CommentAttachment;

namespace TaskTrackerApp.Application.HelperMethods;

public static class CommentEmptyTextValidator
{
    public static bool IsEmpty(
        string request,
        List<int>? KeepAttachmentIds,
        List<AttachmentInputDto>? NewAttachments
        )
    {
        var cleanText = request?.Trim() ?? string.Empty;
        bool isTextEmpty = string.IsNullOrWhiteSpace(cleanText) ||
                           cleanText == "<p><br></p>" ||
                           cleanText == "<p></p>";

        bool hasNoNewFiles = NewAttachments == null || NewAttachments.Any();
        bool hasNoKeptFiles = KeepAttachmentIds == null || KeepAttachmentIds.Any();

        if (isTextEmpty && hasNoNewFiles && hasNoKeptFiles)
        {
            return true;
        }

        return false;
    }

    public static bool IsEmpty(
    string request,
    List<AttachmentInputDto>? Attachments
    )
    {
        var cleanText = request?.Trim() ?? string.Empty;
        bool isTextEmpty = string.IsNullOrWhiteSpace(cleanText) ||
                           cleanText == "<p><br></p>" ||
                           cleanText == "<p></p>";

        if (isTextEmpty && (Attachments == null || Attachments.Any()))
        {
            return true;
        }

        return false;
    }
}