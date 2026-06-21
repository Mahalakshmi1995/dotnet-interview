using Xunit;
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