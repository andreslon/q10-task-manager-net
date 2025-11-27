using Microsoft.EntityFrameworkCore;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;
using Q10.TaskManager.Infrastructure.Data;

namespace Q10.TaskManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PostgreSQLContext _context;

        public UserRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<User>> GetAllUsersAsync()
        {
            var users = _context.Users.AsQueryable();
            return Task.FromResult(users);
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(string id, User user)
        {
            user.Id = id;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
