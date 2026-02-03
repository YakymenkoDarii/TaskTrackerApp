using Microsoft.Azure.Cosmos;
using TaskTrackerApp.Functions.Functions.Data.Cosmos;
using TaskTrackerApp.Functions.Functions.Interfaces.Repositories;

namespace TaskTrackerApp.Functions.Functions.Data.Repositories;

public class CosmosRepository : ICosmosRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;

    private readonly string _backupContainerName = "ArchivedBoards";
    private readonly string _jobsContainerName = "ArchivationJobs";

    public CosmosRepository(CosmosClient cosmosClient, string databaseName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
    }

    private async Task<Container> GetContainerAsync(string containerName)
    {
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);

        var containerResponse = await databaseResponse.Database
            .CreateContainerIfNotExistsAsync(containerName, "/BoardId");

        return containerResponse.Container;
    }

    public async Task UpsertJobAsync(ArchivationJob job)
    {
        var container = await GetContainerAsync(_jobsContainerName);
        await container.UpsertItemAsync(job, new PartitionKey(job.BoardId));
    }

    public async Task CreateBackupAsync(BoardBackupDocument backup)
    {
        var container = await GetContainerAsync(_backupContainerName);
        await container.CreateItemAsync(backup, new PartitionKey(backup.BoardId));
    }

    public async Task<BoardBackupDocument?> GetBackupByBoardIdAsync(int boardId)
    {
        var container = await GetContainerAsync(_backupContainerName);

        var query = new QueryDefinition("SELECT * FROM c WHERE c.BoardId = @boardId")
            .WithParameter("@boardId", boardId);

        using var iterator = container.GetItemQueryIterator<BoardBackupDocument>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(boardId)
            });

        if (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            return response.FirstOrDefault();
        }

        return null;
    }
}