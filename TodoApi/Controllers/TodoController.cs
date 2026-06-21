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
 
        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }
 
        // POST api/todos
        [HttpPost]
        public IActionResult CreateTodo([FromBody] Todo todo)
        {
            try
            {
                _todoService.CreateTodo(todo);
                var created = _todoService.GetTodoById(todo.Id);
                return CreatedAtAction(nameof(GetTodoById), new { id = created!.Id },
                    ApiResponse<Todo>.Ok(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create todo");
                return StatusCode(500, ApiResponse<Todo>.Fail("An unexpected error occurred"));
            }
        }
 
        // GET api/todos
        [HttpGet]
        public IActionResult GetAllTodos()
        {
            try
            {
                var todos = _todoService.GetAllTodos();
                return Ok(ApiResponse<List<Todo>>.Ok(todos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve todos");
                return StatusCode(500, ApiResponse<List<Todo>>.Fail("An unexpected error occurred"));
            }
        }
 
        // GET api/todos/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetTodoById(int id)
        {
            try
            {
                var todo = _todoService.GetTodoById(id);
                if (todo is null)
                    return NotFound(ApiResponse<Todo>.Fail($"Todo with id {id} not found"));
 
                return Ok(ApiResponse<Todo>.Ok(todo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve todo {Id}", id);
                return StatusCode(500, ApiResponse<Todo>.Fail("An unexpected error occurred"));
            }
        }
 
        // PUT api/todos/{id}
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
 
                var updated = _todoService.UpdateTodo(id, todo);
                if (!updated)
                    return NotFound(ApiResponse<Todo>.Fail($"Todo with id {id} not found"));
 
                return Ok(ApiResponse<string>.Ok("Todo updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update todo {Id}", id);
                return StatusCode(500, ApiResponse<Todo>.Fail("An unexpected error occurred"));
            }
        }
 
        // DELETE api/todos/{id}
        [HttpDelete("{id:int}")]
        public IActionResult DeleteTodo(int id)
        {
            try
            {
                var deleted = _todoService.DeleteTodo(id);
                if (!deleted)
                    return NotFound(ApiResponse<Todo>.Fail($"Todo with id {id} not found"));
 
                return Ok(ApiResponse<string>.Ok("Todo deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete todo {Id}", id);
                return StatusCode(500, ApiResponse<Todo>.Fail("An unexpected error occurred"));
            }
        }
    }
}