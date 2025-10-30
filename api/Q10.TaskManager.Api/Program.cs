using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);



// Add Memory Cache

builder.Services.AddServices();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddDatabaseConfiguration();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:80")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Swagger configuration
builder.Services.AddSwaggerConfiguration();

// Add Authentication & Authorization
builder.Services.AddAuthConfiguration(builder.Configuration);

var app = builder.Build();
await builder.Services.DatabaseCreatedAsync();
// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAngularApp");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();