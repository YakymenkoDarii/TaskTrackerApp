using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Persistence.Contexts;
using TaskTrackerApp.Persistence.Repositories;
using TaskTrackerApp.Persistence.UoW;

namespace TaskTrackerApp.Persistence.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContextFactory<TaskTrackerDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<TaskTrackerDbContext>(p =>
            p.GetRequiredService<IDbContextFactory<TaskTrackerDbContext>>().CreateDbContext());

        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}