using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;
using Q10.TaskManager.Infrastructure.Data;

namespace Q10.TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly PostgreSQLContext _context;

        public TaskRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await _context.TaskItems.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            var task = await GetTaskByIdAsync(id);
            if (task == null) return false;
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<TaskItem>> GetAllTasksAsync()
        {
            var tasks = _context.TaskItems.AsQueryable();
            return Task.FromResult(tasks);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(string id)
        {
            var task = await _context.TaskItems
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(string id, TaskItem task)
        {
            task.Id = id;
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }
    }
}
