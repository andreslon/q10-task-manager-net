using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);



// Add Memory Cache

builder.Services.AddServices();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddDatabaseConfiguration();

// Add Swagger configuration
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();
await builder.Services.DatabaseCreatedAsync();
// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseHttpsRedirection();

app.MapControllers();
app.Run();