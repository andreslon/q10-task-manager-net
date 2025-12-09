using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Infrastructure.Data;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public PostgreSQLContext Context { get; set; }
        public TaskRepository(PostgreSQLContext context)
        {
            Context = context;
        }
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await Context.TaskItems.AddAsync(task);
            await Context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
           var task = await GetTaskByIdAsync(id);
            if (task == null) return false;
            Context.TaskItems.Remove(task);
            await Context.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<TaskItem>> GetAllTasksAsync()
        {
            var tasks = Context.TaskItems.AsQueryable();
            return Task.FromResult(tasks);
        }

        public async Task<TaskItem> GetTaskByIdAsync(string id)
        {
            var task = await Context.TaskItems
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(string id, TaskItem task)
        {
            task.Id = id;
            Context.Entry(task).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return task;
        }
    }
}
