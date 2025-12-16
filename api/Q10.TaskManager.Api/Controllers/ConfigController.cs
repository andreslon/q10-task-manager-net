using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfig _config;
        
        public ConfigController(IConfig config)
        {
            _config = config;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            if (_config == null)
            {
                return BadRequest("Configuration service is not available");
            }
            
            return Ok(_config.GetValue("ASPNETCORE_ENVIRONMENT"));
        }
    }
}
