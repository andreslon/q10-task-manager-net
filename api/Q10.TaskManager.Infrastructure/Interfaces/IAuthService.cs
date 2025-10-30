using Q10.TaskManager.Infrastructure.DTOs;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface IAuthService
    {
        //Creacion de usuario
        Task<AuthResponse> RegisterAsync(UserRequest request);
        Task<string> HashPasswordAsync(string password);


        //Generacion de token
        Task<bool> VerifyPasswordAsync(string password, string hash);
        Task<string> GenerateTokenAsync(string userId, string email, string role);

        //Validacion de token

        Task<bool> ValidateTokenAsync(string token);
        //Task<AuthResponse> LoginAsync(LoginRequest request);

    }
}
