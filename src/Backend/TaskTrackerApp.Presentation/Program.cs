using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.DependencyInjection;
using TaskTrackerApp.Database;
using TaskTrackerApp.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

DatabaseInitializer.Initialize(connectionString);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();