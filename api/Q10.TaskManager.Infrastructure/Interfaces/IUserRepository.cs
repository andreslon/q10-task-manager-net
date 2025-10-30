using Q10.TaskManager.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<IQueryable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
    }
}
