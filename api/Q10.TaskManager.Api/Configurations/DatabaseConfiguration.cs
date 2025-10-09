using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Infrastructure.Data;

namespace Q10.TaskManager.Api.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
        {
            services.AddDbContext<PostgreSQLContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"))); 


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
