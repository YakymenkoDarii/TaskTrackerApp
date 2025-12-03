using Microsoft.Extensions.Configuration;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Persistence.Contexts;
using TaskTrackerApp.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

//Task: Add here DbUp implementation

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

//Task: Configure Database and add here a connection string
//builder.Services.AddDbContext<TaskTrackerDbContext>(options =>
//    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICardRepository, CardRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
