using Microsoft.AspNetCore.Mvc;

namespace Q10.TaskManager.Api.Controllers
{
    /// <summary>
    /// Controller para verificar el estado de salud de la aplicación
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica el estado de salud de la aplicación
        /// </summary>
        /// <returns>Estado OK si la aplicación está funcionando correctamente</returns>
        /// <response code="200">La aplicación está funcionando correctamente</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok("OK");
        }
    }
}
