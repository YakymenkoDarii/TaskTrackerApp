using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Auth;
using TaskTrackerApp.Infrastructure.Auth;

namespace TaskTrackerApp.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}