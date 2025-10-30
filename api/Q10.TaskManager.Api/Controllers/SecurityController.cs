using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ILogger<SecurityController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint público - no requiere autenticación
        /// </summary>
        [HttpGet("public")]
        public IActionResult GetPublicInfo()
        {
            return Ok(new { 
                message = "Esta información es pública y no requiere autenticación",
                timestamp = DateTime.UtcNow,
                server = Environment.MachineName
            });
        }

        /// <summary>
        /// Endpoint protegido - requiere autenticación
        /// </summary>
        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetProtectedInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { 
                message = "Esta información es protegida y requiere autenticación",
                user = new { userId, userEmail, userRole },
                timestamp = DateTime.UtcNow,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        /// <summary>
        /// Endpoint solo para administradores
        /// </summary>
        [HttpGet("admin-only")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult GetAdminInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new { 
                message = "Esta información es solo para administradores",
                admin = new { userId, userEmail },
                timestamp = DateTime.UtcNow,
                sensitiveData = "Datos sensibles que solo los administradores pueden ver"
            });
        }

        /// <summary>
        /// Endpoint para usuarios y administradores
        /// </summary>
        [HttpGet("user-or-admin")]
        [Authorize(Policy = "RequireUserRole")]
        public IActionResult GetUserOrAdminInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { 
                message = "Esta información es para usuarios autenticados (User o Admin)",
                user = new { userId, userEmail, userRole },
                timestamp = DateTime.UtcNow,
                permissions = "Tienes permisos para ver esta información"
            });
        }

        /// <summary>
        /// Endpoint que muestra información de seguridad del token
        /// </summary>
        [HttpGet("token-info")]
        [Authorize]
        public IActionResult GetTokenInfo()
        {
            var claims = User.Claims.Select(c => new { 
                Type = c.Type, 
                Value = c.Value,
                Issuer = c.Issuer
            }).ToList();

            var tokenInfo = new
            {
                message = "Información del token JWT actual",
                claims,
                authenticationType = User.Identity?.AuthenticationType,
                isAuthenticated = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name,
                timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Usuario {UserId} consultó información del token", 
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return Ok(tokenInfo);
        }
    }
}
