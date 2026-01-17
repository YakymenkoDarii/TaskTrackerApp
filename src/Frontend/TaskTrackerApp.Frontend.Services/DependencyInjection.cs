using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;
using TaskTrackerApp.Frontend.Services.Services.Auth;
using TaskTrackerApp.Frontend.Services.Services.BoardInvitations;
using TaskTrackerApp.Frontend.Services.Services.BoardMembers;
using TaskTrackerApp.Frontend.Services.Services.Boards;
using TaskTrackerApp.Frontend.Services.Services.CardComments;
using TaskTrackerApp.Frontend.Services.Services.Cards;
using TaskTrackerApp.Frontend.Services.Services.Columns;
using TaskTrackerApp.Frontend.Services.Services.Labels;
using TaskTrackerApp.Frontend.Services.Services.Users;

namespace TaskTrackerApp.Frontend.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["BaseUri"];

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoardsService, BoardsService>();
        services.AddScoped<IColumnsService, ColumnsService>();
        services.AddScoped<ICardsService, CardsService>();
        services.AddScoped<IBoardInvitationsService, BoardInvitationsService>();
        services.AddScoped<IBoardMembersService, BoardMembersService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ICardCommentsService, CardCommentsService>();
        services.AddScoped<ILabelService, LabelService>();

        services.AddBlazoredSessionStorage();
        services.AddScoped<ITokenStorage, TokenStorage>();

        services.AddTransient<AuthMessageHandler>();
        services.AddTransient<CookieHandler>();

        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<CookieHandler>();

        services.AddRefitClient<IBoardsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<IColumnsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<ICardsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<IBoardInvitationsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<IBoardMembersApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<IUsersApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<ICardCommentsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddRefitClient<ILabelApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
            .AddHttpMessageHandler<AuthMessageHandler>();

        return services;
    }
}