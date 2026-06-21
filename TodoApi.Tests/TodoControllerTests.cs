using Moq;
using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TodoApi.Tests;

public class TodoControllerTests
{
    private readonly Mock<ITodoService> _mockService;
    private readonly Mock<ILogger<TodoController>> _mockLogger;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mockService = new Mock<ITodoService>();
        _mockLogger = new Mock<ILogger<TodoController>>();
        _controller = new TodoController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateTodo_Returns201_WhenSuccessful()
    {
        var todo = new Todo { Title = "Test", Description = "Desc" };
        var created = new Todo { Id = 1, Title = "Test", Description = "Desc" };

        _mockService.Setup(s => s.CreateTodoAsync(todo)).Returns(Task.CompletedTask);
        _mockService.Setup(s => s.GetTodoByIdAsync(todo.Id)).ReturnsAsync(created);

        var result = await _controller.CreateTodo(todo) as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);

        var response = result.Value as ApiResponse<Todo>;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(created, response.Data);
    }

    [Fact]
    public async Task GetAllTodos_Returns200_WithList()
    {
        var todos = new List<Todo> { new Todo { Id = 1, Title = "Test" } };
        _mockService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(todos);

        var result = await _controller.GetAllTodos() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApiResponse<List<Todo>>;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(todos, response.Data);
    }

    [Fact]
    public async Task GetTodoById_Returns200_WhenFound()
    {
        var todo = new Todo { Id = 1, Title = "Test" };
        _mockService.Setup(s => s.GetTodoByIdAsync(1)).ReturnsAsync(todo);

        var result = await _controller.GetTodoById(1) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApiResponse<Todo>;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal(todo, response.Data);
    }

    [Fact]
    public async Task GetTodoById_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.GetTodoByIdAsync(9999)).ReturnsAsync((Todo?)null);

        var result = await _controller.GetTodoById(9999) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);

        var response = result.Value as ApiResponse<Todo>;
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotNull(response.Message);
    }

    [Fact]
    public async Task UpdateTodo_Returns200_WhenUpdated()
    {
        var request = new UpdateTodoRequest { Title = "Updated", Description = "Desc", IsCompleted = true };
        _mockService.Setup(s => s.UpdateTodoAsync(1, It.IsAny<Todo>())).ReturnsAsync(true);

        var result = await _controller.UpdateTodo(1, request) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApiResponse<string>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateTodo_Returns404_WhenNotFound()
    {
        var request = new UpdateTodoRequest { Title = "Ghost", Description = "Desc" };
        _mockService.Setup(s => s.UpdateTodoAsync(9999, It.IsAny<Todo>())).ReturnsAsync(false);

        var result = await _controller.UpdateTodo(9999, request) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);

        var response = result.Value as ApiResponse<Todo>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }

    [Fact]
    public async Task DeleteTodo_Returns200_WhenDeleted()
    {
        _mockService.Setup(s => s.DeleteTodoAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTodo(1) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApiResponse<string>;
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task DeleteTodo_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.DeleteTodoAsync(9999)).ReturnsAsync(false);

        var result = await _controller.DeleteTodo(9999) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);

        var response = result.Value as ApiResponse<Todo>;
        Assert.NotNull(response);
        Assert.False(response.Success);
    }
}