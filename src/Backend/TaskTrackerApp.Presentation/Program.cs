using Microsoft.EntityFrameworkCore;
using TaskTrackerApp.Application.Features.Cards.Commands.CreateCard; // Needed to find the Application Assembly
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Application.Interfaces.UoW;
using TaskTrackerApp.Database;
using TaskTrackerApp.Persistence.Contexts;
using TaskTrackerApp.Persistence.Repositories;
using TaskTrackerApp.Persistence.UoW;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContextFactory<TaskTrackerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<TaskTrackerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCardCommandHandler).Assembly));

builder.Services.AddScoped<ICardRepository, CardRepository>();

builder.Services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

DatabaseInitializer.Initialize(connectionString);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();