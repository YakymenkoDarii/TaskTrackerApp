using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using System.Text.Json;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.DTOs.BackupDtos;

namespace TaskTrackerApp.Infrastructure.Services;

public class BoardBackupService : IBoardBackupService
{
    private readonly ICosmosJobTracker _cosmosTracker;
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "board-archives";
    private readonly CosmosClient _cosmosClient;
    private const string DatabaseName = "TaskTrackerDb";

    public BoardBackupService(ICosmosJobTracker cosmosTracker, BlobServiceClient blobServiceClient, CosmosClient cosmosClient)
    {
        _cosmosTracker = cosmosTracker;
        _blobServiceClient = blobServiceClient;
        _cosmosClient = cosmosClient;
    }

    public async Task<BoardBackupDto?> GetBackupAsync(int boardId, CancellationToken ct)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        string blobPath = $"board-{boardId}/data.json";
        var blobClient = containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(ct))
        {
            var job = await _cosmosTracker.GetJobByBoardIdAsync(boardId);
            if (job != null && !string.IsNullOrEmpty(job.BlobUrl))
            {
                blobClient = new BlobClient(new Uri(job.BlobUrl));
                if (!await blobClient.ExistsAsync(ct)) return null;
            }
            else
            {
                return null;
            }
        }

        var download = await blobClient.DownloadContentAsync(ct);
        var json = download.Value.Content.ToString();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        return JsonSerializer.Deserialize<BoardBackupDto>(json, options);
    }

    public async Task DeleteBackupAsync(int boardId, CancellationToken ct)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient("board-archives");
        var blobClient = blobContainer.GetBlobClient($"board-{boardId}/data.json");
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);

        var jobsContainer = _cosmosClient.GetContainer(DatabaseName, "ArchivationJobs");

        await jobsContainer.DeleteItemAsync<object>(
            id: boardId.ToString(),
            partitionKey: new PartitionKey(boardId),
            cancellationToken: ct);

        var archivesContainer = _cosmosClient.GetContainer(DatabaseName, "ArchivedBoards");

        var query = new QueryDefinition("SELECT * FROM c WHERE c.BoardId = @boardId")
            .WithParameter("@boardId", boardId);

        using var iterator = archivesContainer.GetItemQueryIterator<dynamic>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(ct);
            foreach (var item in response)
            {
                string docId = item.id;
                await archivesContainer.DeleteItemAsync<object>(
                    id: docId,
                    partitionKey: new PartitionKey(boardId),
                    cancellationToken: ct);
            }
        }
    }
}