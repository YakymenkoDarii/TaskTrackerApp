using Microsoft.Extensions.DependencyInjection;

namespace TaskTrackerApp.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(AssemblyReference.AssemblyReference.Assembly));

        return services;
    }
}