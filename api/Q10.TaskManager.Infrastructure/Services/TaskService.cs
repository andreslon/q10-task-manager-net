using Q10.TaskManager.Infrastructure.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        public ITaskRepository TaskRepository { get; set; }
        public TaskService(ITaskRepository taskRepository)
        {
            TaskRepository = taskRepository;
        }

        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            if (task == null) {
                throw new ArgumentNullException(nameof(task));
            }
            if (string.IsNullOrEmpty(task.Title))
            {
                throw new ArgumentException("Task title cannot be null or empty");
            }
            if (string.IsNullOrEmpty(task.Description))
            {
                throw new ArgumentException("Task description cannot be null or empty");
            }
            var newTask = await TaskRepository.CreateTaskAsync(task);
            return newTask;
        }

        public async Task<bool> DeleteTask(string id)
        {
            var result=await TaskRepository.DeleteTaskAsync(id);
            return result;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks()
        {
            IQueryable<TaskItem> tasks = await TaskRepository.GetAllTasksAsync();
            if (true)
                tasks = tasks.Where(t => t != null);
            if (true)
                tasks = tasks.Where(t => t != null);
             return tasks.ToList(); 
        }

        public Task<TaskItem> GetTaskById(string id)
        {
            var task = TaskRepository.GetTaskByIdAsync(id);
            return task;
        }

        public Task<TaskItem> UpdateTask(string id, TaskItem task)
        {
            var updatedTask = TaskRepository.UpdateTaskAsync(id, task);
            return updatedTask;
        }
    }
}
