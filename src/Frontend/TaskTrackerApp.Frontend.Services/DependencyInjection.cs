using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Auth;
using TaskTrackerApp.Frontend.Services.Services.Boards;
using TaskTrackerApp.Frontend.Services.Services.Cards;
using TaskTrackerApp.Frontend.Services.Services.Columns;

namespace TaskTrackerApp.Frontend.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoardsService, BoardsService>();
        services.AddScoped<IColumnsService, ColumnsService>();
        services.AddScoped<ICardsService, CardsService>();

        services.AddScoped<ITokenStorage, TokenStorage>();

        services.AddTransient<AuthMessageHandler>();
        services.AddTransient<CookieHandler>();

        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

        return services;
    }
}