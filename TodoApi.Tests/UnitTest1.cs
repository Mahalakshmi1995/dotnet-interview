using Xunit;
using Moq;
using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
 
namespace TodoApi.Tests;
 
// -------------------------------------------------------
// Service tests — use in-memory SQLite, no real DB touched
// -------------------------------------------------------
public class TodoServiceTests : IDisposable
{
    private readonly string _connectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAlive;
    private readonly TodoService _service;
 
    public TodoServiceTests()
    {
        // Keep a persistent connection open so the in-memory DB isn't destroyed between calls
        _keepAlive = new SqliteConnection(_connectionString);
        _keepAlive.Open();
 
        var command = _keepAlive.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Todos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT,
                IsCompleted INTEGER NOT NULL DEFAULT 0 CHECK (IsCompleted IN (0, 1)),
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            )
        ";
        command.ExecuteNonQuery();
 
        _service = new TodoService(_connectionString);
    }
 
    public void Dispose()
    {
        _keepAlive.Dispose();
    }
 
    [Fact]
    public void CreateTodo_ReturnsCreatedTodoWithId()
    {
        var todo = new Todo { Title = "Test", Description = "Test Description", IsCompleted = false };
 
        var result = _service.CreateTodo(todo);
 
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test", result.Title);
    }
 
    [Fact]
    public void GetAllTodos_ReturnsEmptyList_WhenNoTodos()
    {
        var result = _service.GetAllTodos();
 
        Assert.NotNull(result);
        Assert.Empty(result);
    }
 
    [Fact]
    public void GetAllTodos_ReturnsAllCreatedTodos()
    {
        _service.CreateTodo(new Todo { Title = "Todo 1", Description = "D1" });
        _service.CreateTodo(new Todo { Title = "Todo 2", Description = "D2" });
 
        var result = _service.GetAllTodos();
 
        Assert.Equal(2, result.Count);
    }
 
    [Fact]
    public void GetTodoById_ReturnsTodo_WhenExists()
    {
        var created = _service.CreateTodo(new Todo { Title = "Find Me", Description = "Desc" });
 
        var result = _service.GetTodoById(created.Id);
 
        Assert.NotNull(result);
        Assert.Equal("Find Me", result.Title);
    }
 
    [Fact]
    public void GetTodoById_ReturnsNull_WhenNotFound()
    {
        var result = _service.GetTodoById(9999);
 
        Assert.Null(result);
    }
 
    [Fact]
    public void UpdateTodo_ReturnsUpdatedTodo_WhenExists()
    {
        var created = _service.CreateTodo(new Todo { Title = "Original", Description = "Desc" });
        var updated = new Todo { Title = "Updated", Description = "New Desc", IsCompleted = true };
 
        var result = _service.UpdateTodo(created.Id, updated);
 
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Title);
        Assert.True(result.IsCompleted);
    }
 
    [Fact]
    public void UpdateTodo_ReturnsNull_WhenNotFound()
    {
        var todo = new Todo { Title = "Ghost", Description = "Desc" };
 
        var result = _service.UpdateTodo(9999, todo);
 
        Assert.Null(result);
    }
 
    [Fact]
    public void DeleteTodo_ReturnsTrue_WhenDeleted()
    {
        var created = _service.CreateTodo(new Todo { Title = "Delete Me", Description = "Desc" });
 
        var result = _service.DeleteTodo(created.Id);
 
        Assert.True(result);
    }
 
    [Fact]
    public void DeleteTodo_ReturnsFalse_WhenNotFound()
    {
        var result = _service.DeleteTodo(9999);
 
        Assert.False(result);
    }
}
 
// -------------------------------------------------------
// Controller tests — mock ITodoService, no DB needed
// -------------------------------------------------------
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
    public void CreateTodo_Returns201_WhenSuccessful()
    {
        var todo = new Todo { Title = "Test", Description = "Desc" };
        var created = new Todo { Id = 1, Title = "Test", Description = "Desc" };
        _mockService.Setup(s => s.CreateTodo(todo)).Returns(created);
 
        var result = _controller.CreateTodo(todo) as CreatedAtActionResult;
 
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
    }
 
    [Fact]
    public void GetAllTodos_Returns200_WithList()
    {
        var todos = new List<Todo> { new Todo { Id = 1, Title = "Test" } };
        _mockService.Setup(s => s.GetAllTodos()).Returns(todos);
 
        var result = _controller.GetAllTodos() as OkObjectResult;
 
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
 
    [Fact]
    public void GetTodoById_Returns200_WhenFound()
    {
        var todo = new Todo { Id = 1, Title = "Test" };
        _mockService.Setup(s => s.GetTodoById(1)).Returns(todo);
 
        var result = _controller.GetTodoById(1) as OkObjectResult;
 
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
 
    [Fact]
    public void GetTodoById_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.GetTodoById(9999)).Returns((Todo?)null);
 
        var result = _controller.GetTodoById(9999);
 
        Assert.IsType<NotFoundResult>(result);
    }
 
    [Fact]
    public void UpdateTodo_Returns200_WhenUpdated()
    {
        var request = new UpdateTodoRequest { Title = "Updated", Description = "Desc", IsCompleted = true };
        var updated = new Todo { Id = 1, Title = "Updated" };
        _mockService.Setup(s => s.UpdateTodo(1, It.IsAny<Todo>())).Returns(updated);
 
        var result = _controller.UpdateTodo(1, request) as OkObjectResult;
 
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
 
    [Fact]
    public void UpdateTodo_Returns404_WhenNotFound()
    {
        var request = new UpdateTodoRequest { Title = "Ghost", Description = "Desc" };
        _mockService.Setup(s => s.UpdateTodo(9999, It.IsAny<Todo>())).Returns((Todo?)null);
 
        var result = _controller.UpdateTodo(9999, request);
 
        Assert.IsType<NotFoundResult>(result);
    }
 
    [Fact]
    public void DeleteTodo_Returns204_WhenDeleted()
    {
        _mockService.Setup(s => s.DeleteTodo(1)).Returns(true);
 
        var result = _controller.DeleteTodo(1);
 
        Assert.IsType<NoContentResult>(result);
    }
 
    [Fact]
    public void DeleteTodo_Returns404_WhenNotFound()
    {
        _mockService.Setup(s => s.DeleteTodo(9999)).Returns(false);
 
        var result = _controller.DeleteTodo(9999);
 
        Assert.IsType<NotFoundResult>(result);
    }
}
 