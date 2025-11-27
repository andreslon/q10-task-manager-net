using Q10.TaskManager.Application.Interfaces;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;

namespace Q10.TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (string.IsNullOrEmpty(task.Title))
            {
                throw new ArgumentException("Task title cannot be null or empty");
            }
            var newTask = await _taskRepository.CreateTaskAsync(task);
            return newTask;
        }

        public async Task<bool> DeleteTask(string id)
        {
            var result = await _taskRepository.DeleteTaskAsync(id);
            return result;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks()
        {
            IQueryable<TaskItem> tasks = await _taskRepository.GetAllTasksAsync();
            return tasks.ToList();
        }

        public async Task<TaskItem?> GetTaskById(string id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            return task;
        }

        public async Task<TaskItem> UpdateTask(string id, TaskItem task)
        {
            var updatedTask = await _taskRepository.UpdateTaskAsync(id, task);
            return updatedTask;
        }
    }
}

