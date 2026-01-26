namespace TaskTrackerApp.Application.Interfaces.BlobStorage;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string containerName, string blobName, string contentType);

    Task DeleteAsync(string containerName, string blobName);

    Task<Stream?> DownloadAsync(string containerName, string blobName);
}