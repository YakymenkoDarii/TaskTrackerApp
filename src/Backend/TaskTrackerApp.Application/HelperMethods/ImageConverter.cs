using System.Text.RegularExpressions;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Domain.Constants;
using TaskTrackerApp.Domain.Entities;

namespace TaskTrackerApp.Application.HelperMethods;

public static class ImageConverter
{
    public static async Task<string> UploadEmbeddedImagesAsync(
    string htmlContent,
    int cardId,
    int commentId,
    ICollection<CommentAttachment> attachmentsCollection,
    IBlobStorageService _blobService)
    {
        if (string.IsNullOrEmpty(htmlContent)) return htmlContent;
        var regex = new Regex(@"src=""data:image/(?<ext>.+?);base64,(?<data>.+?)""");
        var matches = regex.Matches(htmlContent);

        foreach (Match match in matches)
        {
            var extension = match.Groups["ext"].Value;
            var base64Data = match.Groups["data"].Value;

            var imageBytes = Convert.FromBase64String(base64Data);

            var storedName = $"{Guid.NewGuid()}.{extension}";
            var blobPath = $"card-{cardId}/comment-{commentId}/{storedName}";
            var contentType = $"image/{extension}";

            using var stream = new MemoryStream(imageBytes);

            var url = await _blobService.UploadAsync(
                stream,
                BlobContainerNames.CommentAttachments,
                blobPath,
                contentType
            );
            htmlContent = htmlContent.Replace(match.Value, $@"src=""{url}""");

            attachmentsCollection.Add(new CommentAttachment
            {
                FileName = $"embedded-image.{extension}",
                StoredFileName = storedName,
                Url = url,
                ContentType = contentType,
                Size = imageBytes.Length,
                CommentId = commentId
            });
        }

        return htmlContent;
    }
}