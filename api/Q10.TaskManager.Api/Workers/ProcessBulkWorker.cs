using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Api.Workers
{
    public class ProcessBulkWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProcessBulkWorker> _logger;

        public ProcessBulkWorker(IServiceProvider serviceProvider, ILogger<ProcessBulkWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ProcessBulkWorker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var rabbitMQRepository = scope.ServiceProvider.GetRequiredService<IRabbitMQRepository>();
                        var processBulkService = scope.ServiceProvider.GetRequiredService<IProcessBulkService>();

                        await rabbitMQRepository.StartConsumingAsync<TaskBulkCommand>(
                            "task-bulk-queue", 
                            processBulkService.ProcessBulkCommand);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ProcessBulkWorker");
                    await Task.Delay(5000, stoppingToken); // Wait 5 seconds before retrying
                }
            }

            _logger.LogInformation("ProcessBulkWorker stopped");
        }
    }
}
