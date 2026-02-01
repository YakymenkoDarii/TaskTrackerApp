using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Application.Features.Behaviors.Board;

namespace TaskTrackerApp.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.AssemblyReference.Assembly);
            cfg.AddOpenBehavior(typeof(BoardArchivedValidationBehavior<,>));
        });
        return services;
    }
}