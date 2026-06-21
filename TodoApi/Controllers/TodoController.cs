using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;
        public TodoController(ITodoService todoService,ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateTodo([FromBody] Todo todo)
        {
            try
            {
                var result = _todoService.CreateTodo(todo);
                return CreatedAtAction(nameof(GetTodoById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Failed to create todo");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet]
        public IActionResult GetAllTodos()
        {
           
            try
            {
                var todos = _todoService.GetAllTodos();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve todo ");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTodoById(int id)
        {
            try
            {
                var todo = _todoService.GetTodoById(id);
                return todo is null ? NotFound() : Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve todo {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            try
            {
                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = request.IsCompleted
                };
 
                var result = _todoService.UpdateTodo(id, todo);
                return result is null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update todo {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }
 
        [HttpDelete("{id:int}")]
        public IActionResult DeleteTodo(int id)
        {
            try
            {
                var deleted = _todoService.DeleteTodo(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete todo {Id}", id);
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
