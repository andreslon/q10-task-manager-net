using Q10.TaskManager.Api.Workers;
using Q10.TaskManager.Infrastructure.Interfaces;
using Q10.TaskManager.Infrastructure.Repositories;
using Q10.TaskManager.Application.Services;
using Q10.UserManager.Infrastructure.Repositories;

namespace Q10.TaskManager.Api.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Add services to the container.
            #region Repositories
            // Register both config repositories - EnvironmentRepository for env vars, SettingsRepository for appsettings
            services.AddScoped<EnvironmentRepository>();
            services.AddScoped<SettingsRepository>();
            services.AddScoped<IConfig, EnvironmentRepository>(); // Use EnvironmentRepository for Kubernetes env vars
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();

            #endregion

            #region Services
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAuthService, AuthService>();

            // CQRS Services
            services.AddScoped<ITaskBulkCommandService, TaskBulkCommandService>();
            services.AddScoped<ITaskBulkQueryService, TaskBulkQueryService>();

            // RabbitMQ Services
            services.AddScoped<IProcessBulkService, ProcessBulkService>();
            services.AddHostedService<ProcessBulkWorker>();
            #endregion

            return services;
        }
    }
}
