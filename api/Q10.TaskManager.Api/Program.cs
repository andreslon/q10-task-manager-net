using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);



// Add Memory Cache

builder.Services.AddServices();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddDatabaseConfiguration();
builder.Services.AddAuthConfiguration();

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

var app = builder.Build();
// await builder.Services.DatabaseCreatedAsync(); // Comentado temporalmente para pruebas
// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration(app.Environment);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Use CORS
app.UseCors("AllowAngularApp");

app.MapControllers();
app.Run();