using Q10.TaskManager.Application.DTOs.Requests;
using Q10.TaskManager.Application.DTOs.Responses;

namespace Q10.TaskManager.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRequest request);
        Task<string> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string password, string hash);
        Task<string> GenerateTokenAsync(string userId, string email, string role);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}

