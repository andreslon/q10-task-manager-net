using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Infrastructure.Services
{
    public class TaskBulkCommandService : ITaskBulkCommandService
    {
        private readonly IRabbitMQRepository _rabbitMQRepository;

        public TaskBulkCommandService(IRabbitMQRepository rabbitMQRepository)
        {
            _rabbitMQRepository = rabbitMQRepository;
        }

        public async Task<string> ProcessBulkTasksAsync(List<TaskBulkRequest> tasks)
        {
            var command = new TaskBulkCommand
            {
                Tasks = tasks
            };

            await _rabbitMQRepository.PublishAsync(command, "task-bulk-queue");

            return command.Id;
        }
    }
}
