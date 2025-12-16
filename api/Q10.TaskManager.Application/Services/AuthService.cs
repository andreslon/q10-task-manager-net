using Microsoft.IdentityModel.Tokens;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Q10.TaskManager.Application.Services
{
    public class AuthService : IAuthService
    {
        public IUserRepository UserRepository { get; set; }
        public AuthService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }
        public async Task<string> GenerateTokenAsync(string userId, string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role),
                new("sub", userId.ToString()),
                new("jti", Guid.NewGuid().ToString()),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer =  "Q10TaskManager",
                Audience = "Q10TaskManagerUsers",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> HashPasswordAsync(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public async Task<AuthResponse> RegisterAsync(UserRequest request)
        {
            var users = await UserRepository.GetAllUsersAsync();
            if (users.Where(x => x.Username == request.Username).Any())
            {
                throw new Exception("Username already exists");
            }
            if (users.Where(x => x.Email == request.Email).Any())
            {
                throw new Exception("Email already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = await HashPasswordAsync(request.Password),
                Role = "User"
            };
            user = await UserRepository.CreateUserAsync(user);

            return new AuthResponse
            { 
                Token = await GenerateTokenAsync(user.Id, user.Email, user.Role),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            };
        }

        public Task<bool> VerifyPasswordAsync(string password, string hash)
        {
            try
            {
                return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var users = await UserRepository.GetAllUsersAsync();
            var user = users.FirstOrDefault(x => x.Username == request.Username);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var isValidPassword = await VerifyPasswordAsync(request.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return new AuthResponse
            {
                Token = await GenerateTokenAsync(user.Id, user.Email, user.Role),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            };
        }
    }
}
