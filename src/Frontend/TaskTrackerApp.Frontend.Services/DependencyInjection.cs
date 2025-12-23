using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services;
using TaskTrackerApp.Frontend.Services.Services.Auth;

namespace TaskTrackerApp.Frontend.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ISessionCacheService, SessionCacheService>();

        services.AddTransient<AuthHeaderHandler>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoardsService, BoardsService>();

        services.AddScoped<AuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<AuthStateProvider>());

        return services;
    }
}