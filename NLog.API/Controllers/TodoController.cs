using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private static readonly List<string> todos = new List<string> { "Wake up", "Eat", "Program", "Sleep" };
        private readonly ILogger<TodoController> _logger;

        public TodoController(ILogger<TodoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogDebug("This is a debug message");
                _logger.LogInformation("This is an info message");
                _logger.LogWarning("This is a warning message");
                _logger.LogError(new Exception(), "This is an error message");

                _logger.LogInformation("Retrieving todo items: {todos}", JsonConvert.SerializeObject(todos));

                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todo items.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            try
            {
                if (id < 0 || id >= todos.Count)
                    return NotFound();

                var todo = todos[id];
                _logger.LogInformation("Retrieving todo item with ID {id}: {todo}", id, todo);

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving todo item with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] string todo)
        {
            try
            {
                todos.Add(todo);
                _logger.LogInformation("Created a new todo item: {todo}", todo);

                return CreatedAtAction(nameof(Get), new { id = todos.Count - 1 }, todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new todo item.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string todo)
        {
            try
            {
                if (id < 0 || id >= todos.Count)
                    return NotFound();

                todos[id] = todo;
                _logger.LogInformation("Updated todo item with ID {id}: {todo}", id, todo);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating todo item with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id < 0 || id >= todos.Count)
                    return NotFound();

                var deletedTodo = todos[id];
                todos.RemoveAt(id);
                _logger.LogInformation("Deleted todo item with ID {id}: {deletedTodo}", id, deletedTodo);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting todo item with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
