using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.BlobStorage;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Infrastructure.Auth;
using TaskTrackerApp.Infrastructure.BlobStorage;
using TaskTrackerApp.Infrastructure.Jobs;
using TaskTrackerApp.Infrastructure.Services;

namespace TaskTrackerApp.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddScoped<IInvitationNotifier, InvitationNotifier>();
        services.AddScoped<IBoardNotifier, BoardNotifier>();
        services.AddScoped<ICardNotifier, CardNotifier>();

        services.AddScoped<IArchivalSyncJob, ArchivalSyncJob>();

        return services;
    }
}