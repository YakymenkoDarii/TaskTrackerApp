using Microsoft.Azure.Cosmos;

using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Domain.Jobs;

namespace TaskTrackerApp.Infrastructure.Jobs;

public class CosmosJobTracker : ICosmosJobTracker
{
    private readonly Container _container;

    public CosmosJobTracker(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        var database = cosmosClient.GetDatabase(databaseName);
        _container = database.GetContainer(containerName);
    }

    public async Task CreateJobAsync(ArchivationJob job)
    {
        await _container.CreateItemAsync(job, new PartitionKey(job.BoardId));
    }

    public async Task UpdateJobAsync(ArchivationJob job)
    {
        await _container.UpsertItemAsync(job, new PartitionKey(job.BoardId));
    }

    public async Task DeleteJobByBoardIdAsync(int boardId)
    {
        var job = await GetJobByBoardIdAsync(boardId);

        if (job != null)
        {
            await _container.DeleteItemAsync<ArchivationJob>(
                job.id,
                new PartitionKey(boardId));
        }
    }

    public async Task<ArchivationJob?> GetJobByBoardIdAsync(int boardId)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.BoardId = @boardId")
            .WithParameter("@boardId", boardId);

        using var iterator = _container.GetItemQueryIterator<ArchivationJob>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(boardId)
            }
        );

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();

            var item = response.FirstOrDefault();
            if (item != null)
            {
                return item;
            }
        }

        return null;
    }
}