using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskTrackerApp.Application.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Common;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Functions.Functions.Stub;
using TaskTrackerApp.Infrastructure.BlobStorage;
using TaskTrackerApp.Infrastructure.Jobs;
using TaskTrackerApp.Persistence.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddApplication();
        services.AddPersistence(configuration);

        services.RemoveAll<ICurrentUserService>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddScoped<IPasswordHasher, StubPasswordHasher>();
        services.AddScoped<ITokenService, StubTokenService>();
        services.AddScoped<ICurrentUserService, StubCurrentUserService>();

        services.AddScoped<StubNotifier>();
        services.AddScoped<IBoardNotifier>(p => p.GetRequiredService<StubNotifier>());
        services.AddScoped<ICardNotifier>(p => p.GetRequiredService<StubNotifier>());
        services.AddScoped<IInvitationNotifier>(p => p.GetRequiredService<StubNotifier>());
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddScoped(x => new BlobServiceClient(
            configuration["AzureWebJobsStorage"],
            new BlobClientOptions(BlobClientOptions.ServiceVersion.V2023_11_03)
        ));

        services.AddScoped(x => new CosmosClient(
            configuration["CosmosDbConnection"]));

        services.AddScoped<ICosmosJobTracker>(s =>
            new CosmosJobTracker(
                s.GetRequiredService<CosmosClient>(),
                "TaskTrackerDb",
                "ArchivationJobs"));

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();