using Microsoft.AspNetCore.Mvc;
using Q10.TaskManager.Infrastructure.Data;
using Q10.TaskManager.Infrastructure.Entities;
using Q10.TaskManager.Infrastructure.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Q10.TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        public IConfig Configuration { get; set; }
        public ICacheRepository CacheRepository { get; set; }
        public PostgreSQLContext Context { get; set; }
        public TasksController(IConfig configuration, ICacheRepository cacheRepository, PostgreSQLContext postgreSQLContext)
        {
            Configuration = configuration;
            CacheRepository = cacheRepository;
            Context = postgreSQLContext;
        }
        // GET: api/<TasksController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TasksController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TasksController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            await Context.TaskItems.AddAsync(new 
                TaskItem { Title = value, Description = "Descripción de " + value 
            });
            await Context.SaveChangesAsync();

            return Ok();
            //CacheRepository.Set($"task_{Guid.NewGuid}", value);
        }

        // PUT api/<TasksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
