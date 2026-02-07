using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TaskTrackerApp.Application.DependencyInjection;
using TaskTrackerApp.Application.Interfaces.Jobs;
using TaskTrackerApp.Database;
using TaskTrackerApp.Domain.Settings;
using TaskTrackerApp.Frontend.Domain.Constants;
using TaskTrackerApp.Infrastructure.DependencyInjection;
using TaskTrackerApp.Infrastructure.Hubs;
using TaskTrackerApp.Infrastructure.Jobs;
using TaskTrackerApp.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped(x => new CosmosClient(
    builder.Configuration.GetConnectionString("CosmosDbConnection") ??
    builder.Configuration["CosmosDbConnection"]
));

builder.Services.AddScoped<ICosmosJobTracker>(s =>
    new CosmosJobTracker(
        s.GetRequiredService<CosmosClient>(),
        "TaskTrackerDb",
        "ArchivedBoards"));

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(builder.Configuration["BaseUri"])
    );
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("authLimiter", limiter =>
    {
        limiter.PermitLimit = 5;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddTransient(x =>
    new BlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

DatabaseInitializer.Initialize(connectionString);

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<IArchivalSyncJob>(
        "nightly-board-archival",
        job => job.RunAsync(),
        Cron.Daily(3));
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("authLimiter");

app.MapHub<InvitationHub>(HubRoutes.Invitations);
app.MapHub<BoardHub>(HubRoutes.Board);
app.MapHub<CardHub>(HubRoutes.Card);

app.Run();