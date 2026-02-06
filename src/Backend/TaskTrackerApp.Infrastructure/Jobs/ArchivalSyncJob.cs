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

        string queueName = _config["ServiceBus:QueueName"];

        await using var client = new ServiceBusClient(connectionString);
        await using var sender = client.CreateSender(queueName);

        ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();

        try
        {
            foreach (var id in boardIds)
            {
                var message = new ServiceBusMessage(id.ToString());

                if (!batch.TryAddMessage(message))
                {
                    await sender.SendMessagesAsync(batch);

                    batch.Dispose();

                    batch = await sender.CreateMessageBatchAsync();

                    if (!batch.TryAddMessage(message))
                    {
                        throw new Exception("Message is too large to fit in an empty batch.");
                    }
                }
            }

            if (batch.Count > 0)
            {
                await sender.SendMessagesAsync(batch);
            }
        }
        finally
        {
            batch?.Dispose();
        }
    }
}