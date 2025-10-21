using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Infrastructure.Services;
using Q10.TaskManager.Api.Workers;

namespace Q10.TaskManager.Api.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Add services to the container.
            #region Repositories
            services.AddScoped<IConfig, SettingsRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            #endregion

            #region Services
            services.AddScoped<ITaskService, TaskService>();
            
            // CQRS Services
            services.AddScoped<ITaskBulkCommandService, TaskBulkCommandService>();
            services.AddScoped<ITaskBulkQueryService, TaskBulkQueryService>();
            
            // RabbitMQ Services
            services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
            services.AddScoped<IProcessBulkService, ProcessBulkService>();
            services.AddHostedService<ProcessBulkWorker>();
            #endregion

            return services;
        }
    }
}
