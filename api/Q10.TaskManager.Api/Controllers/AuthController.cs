using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Application.DTOs.Requests;
using Q10.TaskManager.Application.DTOs.Responses;
using Q10.TaskManager.Application.Interfaces;

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthService AuthService { get; set; }
        public AuthController(IAuthService authService)
        {
            AuthService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRequest user)
        {
            try
            {
                var jwt = await AuthService.RegisterAsync(user);
                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var response = await AuthService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
