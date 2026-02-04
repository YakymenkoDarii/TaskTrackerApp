using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.UoW;

namespace TaskTrackerApp.Infrastructure.Jobs;

public class ArchivalSyncJob : IArchivalSyncJob
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IConfiguration _config;

    public ArchivalSyncJob(IUnitOfWorkFactory uowFactory, IConfiguration config)
    {
        _uowFactory = uowFactory;
        _config = config;
    }

    public async Task RunAsync()
    {
        IEnumerable<int> boardIds;

        using (var uow = _uowFactory.Create())
        {
            boardIds = await uow.BoardRepository.GetBoardIdsToArchiveAsync();
        }

        if (!boardIds.Any()) return;

        string connectionString = _config.GetConnectionString("ServiceBusConnection");
        string queueName = "export-board-queue";

        await using var client = new ServiceBusClient(connectionString);
        await using var sender = client.CreateSender(queueName);

        using ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();

        foreach (var id in boardIds)
        {
            var message = new ServiceBusMessage(id.ToString());

            if (!batch.TryAddMessage(message))
            {
                await sender.SendMessagesAsync(batch);
                using var newBatch = await sender.CreateMessageBatchAsync();
                newBatch.TryAddMessage(message);
            }
        }

        await sender.SendMessagesAsync(batch);
    }
}