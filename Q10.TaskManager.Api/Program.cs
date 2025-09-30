using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IConfig, SettingsRepository>();

// Add Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheRepository, CacheRepository>();

builder.Services.AddControllers();

// Add Swagger configuration
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseHttpsRedirection();

app.MapControllers();
app.Run();