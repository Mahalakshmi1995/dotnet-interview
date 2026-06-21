using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Manages Todo items
    /// </summary>
    [ApiController]
    [Route("api/todos")]
    [Produces("application/json")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>Creates a new todo item</summary>
        /// <response code="201">Todo created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 201)]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 400)]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 500)]
        public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
        {
            try
            {
                await _todoService.CreateTodoAsync(todo);
                var created = await _todoService.GetTodoByIdAsync(todo.Id);
                return CreatedAtAction(nameof(GetTodoById), new { id = created!.Id },
                    ApiResponse<Todo>.Ok(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create todo");
                return StatusCode(500, ApiResponse<Todo>.Fail("An unexpected error occurred"));
            }
        }

        /// <summary>Retrieves all todo items</summary>
        /// <response code="200">Returns the list of todos</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<Todo>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<List<Todo>>), 500)]
        public async Task<IActionResult> GetAllTodos()
        {
            try
            {
                var todos = await _todoService.GetAllTodosAsync();
                return Ok(ApiResponse<List<Todo>>.Ok(todos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve todos");
                return StatusCode(500, ApiResponse<List<Todo>>.Fail("An unexpected error occurred"));
            }
        }

        /// <summary>Retrieves a todo item by ID</summary>
        /// <response code="200">Returns the todo item</response>
        /// <response code="404">Todo not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 200)]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 404)]
        [ProducesResponseType(typeof(ApiResponse<Todo>), 500)]
        public async Task<IActionResult> GetTodoById(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
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

        /// <summary>Updates an existing todo item</summary>
        /// <response code="200">Todo updated successfully</response>
        /// <response code="404">Todo not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            try
            {
                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = request.IsCompleted
                };

                var updated = await _todoService.UpdateTodoAsync(id, todo);
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

        /// <summary>Soft deletes a todo item</summary>
        /// <response code="200">Todo deleted successfully</response>
        /// <response code="404">Todo not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var deleted = await _todoService.DeleteTodoAsync(id);
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