using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskBulkController : ControllerBase
    {
        private readonly ITaskBulkCommandService _commandService;
        private readonly ITaskBulkQueryService _queryService;

        public TaskBulkController(ITaskBulkCommandService commandService, ITaskBulkQueryService queryService)
        {
            _commandService = commandService;
            _queryService = queryService;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> CreateBulkTasks([FromBody] List<TaskBulkRequest> tasks)
        {
            if (tasks == null || !tasks.Any())
            {
                return BadRequest("Tasks list cannot be empty");
            }

            try
            {
                var commandId = await _commandService.ProcessBulkTasksAsync(tasks);
                
                // Retornar los IDs de las tareas (en una implementación real, esto vendría del resultado)
                var taskIds = tasks.Select((_, index) => $"{commandId}-{index}").ToList();
                
                return Ok(taskIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing bulk tasks: {ex.Message}");
            }
        }

        [HttpGet("{commandId}/results")]
        public async Task<ActionResult<List<TaskBulkResponse>>> GetBulkTaskResults(string commandId)
        {
            try
            {
                var results = await _queryService.GetBulkTaskResultsAsync(commandId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving results: {ex.Message}");
            }
        }
    }
}
