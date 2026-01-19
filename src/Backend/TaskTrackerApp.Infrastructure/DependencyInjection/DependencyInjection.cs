using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Application.Interfaces.Services;
using TaskTrackerApp.Infrastructure.Auth;
using TaskTrackerApp.Infrastructure.Services;

namespace TaskTrackerApp.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IInvitationNotifier, InvitationNotifier>();

        return services;
    }
}