using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Infrastructure.Data;

namespace Q10.TaskManager.Api.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
        {
            services.AddDbContext<PostgreSQLContext>(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Usar SQLite para pruebas si no hay PostgreSQL
                    options.UseSqlite("Data Source=test.db");
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            });

            return services;
        }

        public static async Task DatabaseCreatedAsync(this IServiceCollection services)
        {
           using var serviceProvider = services.BuildServiceProvider();
            var pgsqlContext = serviceProvider.GetRequiredService<PostgreSQLContext>();
            await pgsqlContext.Database.EnsureCreatedAsync();
        }
    }
}
