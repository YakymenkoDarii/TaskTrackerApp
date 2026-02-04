using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Functions.Functions.Data.Context;
using TaskTrackerApp.Functions.Functions.Data.Repositories;
using TaskTrackerApp.Functions.Functions.Interfaces.Repositories;
using TaskTrackerApp.Functions.Functions.Interfaces.Services;
using TaskTrackerApp.Functions.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var sqlConnectionString = configuration["DefaultConnection"]
                                  ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ArchivalDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddScoped(x => new BlobServiceClient(
            configuration["AzureWebJobsStorage"],
            new BlobClientOptions(BlobClientOptions.ServiceVersion.V2023_11_03)
        ));

        services.AddScoped(s =>
        {
            var connectionString = configuration["CosmosDbConnection"];
            return new CosmosClient(connectionString);
        });

        services.AddScoped<ICosmosRepository>(s =>
            new CosmosRepository(
                s.GetRequiredService<CosmosClient>(),
                databaseName: "TaskTrackerDb"
            ));

        services.AddScoped<IBoardRepository, BoardRepository>();

        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBoardArchivalService, BoardArchivalService>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();