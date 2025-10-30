using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Infrastructure.Data;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PostgreSQLContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(PostgreSQLContext context, IAuthService authService, ILogger<UsersController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Crear un usuario administrador
        /// </summary>
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el usuario ya existe
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "El usuario ya existe con este email" });
                }

                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = await _authService.HashPasswordAsync(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario administrador creado: {Email}", request.Email);

                return Ok(new { 
                    message = "Usuario administrador creado exitosamente",
                    user = new { 
                        id = user.Id, 
                        username = user.Username, 
                        email = user.Email, 
                        role = user.Role 
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario administrador");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear un usuario normal
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el usuario ya existe
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "El usuario ya existe con este email" });
                }

                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = await _authService.HashPasswordAsync(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario normal creado: {Email}", request.Email);

                return Ok(new { 
                    message = "Usuario normal creado exitosamente",
                    user = new { 
                        id = user.Id, 
                        username = user.Username, 
                        email = user.Email, 
                        role = user.Role 
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario normal");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Listar todos los usuarios (solo administradores)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new {
                        id = u.Id,
                        username = u.Username,
                        email = u.Email,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        role = u.Role,
                        isActive = u.IsActive,
                        createdAt = u.CreatedAt,
                        lastLoginAt = u.LastLoginAt
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener información del usuario actual
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var user = await _context.Users
                    .Where(u => u.Id == id)
                    .Select(u => new {
                        id = u.Id,
                        username = u.Username,
                        email = u.Email,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        role = u.Role,
                        isActive = u.IsActive,
                        createdAt = u.CreatedAt,
                        lastLoginAt = u.LastLoginAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario actual");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
