using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Infrastructure.Services
{
    public class TaskBulkQueryService : ITaskBulkQueryService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskBulkQueryService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskBulkResponse>> GetBulkTaskResultsAsync(string commandId)
        {
            // En una implementación real, esto consultaría una tabla de resultados
            // Por simplicidad, retornamos una lista vacía
            await Task.CompletedTask;
            return new List<TaskBulkResponse>();
        }
    }
}
