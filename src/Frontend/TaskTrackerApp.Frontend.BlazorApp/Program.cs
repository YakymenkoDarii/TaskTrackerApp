using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using TaskTrackerApp.Frontend.BlazorApp;
using TaskTrackerApp.Frontend.Services;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["BaseUri"];

builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddProjectServices();

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddRefitClient<IBoardsApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl!))
    .AddHttpMessageHandler<AuthMessageHandler>();

await builder.Build().RunAsync();