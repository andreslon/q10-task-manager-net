using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Infrastructure.Data;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.UserManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public PostgreSQLContext Context { get; set; }
        public UserRepository(PostgreSQLContext context)
        {
            Context = context;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<User>> GetAllUsersAsync()
        {
            var users = Context.Users.AsQueryable();
            return Task.FromResult(users);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await Context.Users
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(string id, User user)
        {
            user.Id = id;
            Context.Entry(user).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return user;
        }
    }
}
