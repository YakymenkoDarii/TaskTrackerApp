using MudBlazor.Services;
using Refit;
using TaskTrackerApp.Frontend.Services;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces;
using TaskTrackerApp.Frontend.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["BaseUri"];

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!));

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();