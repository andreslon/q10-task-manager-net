using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IQueryable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
    }
}


