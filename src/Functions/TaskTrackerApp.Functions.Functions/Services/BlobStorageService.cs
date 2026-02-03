using Azure.Storage.Blobs;
using System.Text;
using TaskTrackerApp.Functions.Functions.Interfaces.Services;

namespace TaskTrackerApp.Functions.Functions.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadBackupAsync<T>(T data, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("board-archives");
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        var json = System.Text.Json.JsonSerializer.Serialize(data);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}