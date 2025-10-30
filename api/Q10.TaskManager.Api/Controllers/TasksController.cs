using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Infrastructure.Data;
using Q10.TaskManager.Infrastructure.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;
using System.Diagnostics.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TasksController : ControllerBase
    {
        public IConfig Configuration { get; set; }
        public ITaskService TaskService { get; set; }
        public TasksController(IConfig configuration, ITaskService taskService)
        {
            Configuration = configuration;
            TaskService = taskService;
        }
        /// <summary>
        /// Creates a new task based on the provided task details.
        /// </summary>
        /// <param name="task">The task details to create, provided in the request body.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see cref="OkObjectResult"/>
        /// with the created task if successful,  or <see cref="BadRequestObjectResult"/> with an error message if the
        /// operation fails.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            try
            {
                var createdTask = await TaskService.CreateTask(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(task);
        }
        /// <summary>
        /// Retrieves all tasks from the task service.
        /// </summary>
        /// <remarks>This method calls the task service to retrieve all tasks and returns the result as an
        /// HTTP response. If an error occurs during the operation, a bad request response is returned  with the
        /// exception message.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing an HTTP 200 response with a collection of  <see cref="TaskItem"/>
        /// objects if the operation is successful, or an HTTP 400 response  with an error message if an exception
        /// occurs.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            IEnumerable<TaskItem> tasks;
            try
            {
                tasks = await TaskService.GetAllTasks();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(tasks);
        }
        /// <summary>
        /// Retrieves a task by its unique identifier.  
        /// </summary>
        /// <remarks>This method returns an HTTP 200 OK response with the task data if the operation is
        /// successful.  If an error occurs, such as the task not being found or an invalid identifier, an HTTP 400 Bad
        /// Request  response is returned with the error message.</remarks>
        /// <param name="id">The unique identifier of the task to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the task with the specified identifier if found,  or a bad request
        /// response if an error occurs.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            TaskItem task;
            try
            {
                task = await TaskService.GetTaskById(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(task);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TaskItem task)
        {
            TaskItem updatedTask;
            try
            {
                updatedTask = await TaskService.UpdateTask(id, task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(updatedTask);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            bool isDeleted;
            try
            {
                isDeleted = await TaskService.DeleteTask(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(isDeleted);
        }

    }
}
