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
                var taskIds = await _commandService.ProcessBulkTasksAsync(tasks);
                return Ok(taskIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing bulk tasks: {ex.Message}");
            }
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskBulkResponse>> GetTaskById(string taskId)
        {
            try
            {
                var result = await _queryService.GetTaskByIdAsync(taskId);
                if (result == null)
                {
                    return NotFound($"Task with ID {taskId} not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving task: {ex.Message}");
            }
        }
    }
}
