using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using StackExchange.Redis;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Domain.Settings;
using TaskTrackerApp.Infrastructure.Auth;
using TaskTrackerApp.Infrastructure.BlobStorage;
using TaskTrackerApp.Infrastructure.Jobs;
using TaskTrackerApp.Infrastructure.Services;

namespace TaskTrackerApp.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureAiSettings>(configuration.GetSection("AzureAiSettings"));

        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBoardBackupService, BoardBackupService>();

        services.AddScoped<IChatHistoryService, ChatHistoryService>();
        services.AddScoped<IFaqService, FaqService>();

        services.AddScoped<IInvitationNotifier, InvitationNotifier>();
        services.AddScoped<IBoardNotifier, BoardNotifier>();
        services.AddScoped<ICardNotifier, CardNotifier>();

        services.AddScoped<IArchivalSyncJob, ArchivalSyncJob>();

        services.AddScoped<IKernelMemory>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<AzureAiSettings>>().Value;

            return new KernelMemoryBuilder()
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
                {
                    APIKey = settings.ApiKey,
                    Endpoint = settings.Endpoint,
                    Deployment = "gpt-4o-mini",
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey
                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                {
                    APIKey = settings.ApiKey,
                    Endpoint = settings.Endpoint,
                    Deployment = "text-embedding-3-small",
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey
                })
                .WithAzureAISearchMemoryDb(new AzureAISearchConfig
                {
                    Endpoint = settings.SearchEndpoint,
                    APIKey = settings.SearchApiKey,
                    Auth = AzureAISearchConfig.AuthTypes.APIKey
                })
                .Build();
        });

        return services;
    }
}