using Moq;
using TodoApi.Services;
using TodoApi.Repositories;
using TodoApi.Models;

namespace TodoApi.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepository;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _mockRepository = new Mock<ITodoRepository>();
        _service = new TodoService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateTodoAsync_CallsRepositoryCreate()
    {
        var todo = new Todo { Title = "Test", Description = "Desc" };

        await _service.CreateTodoAsync(todo);

        _mockRepository.Verify(r => r.CreateAsync(todo), Times.Once);
    }

    [Fact]
    public async Task GetAllTodosAsync_CallsRepositoryGetAll_AndReturnsResult()
    {
        var todos = new List<Todo> { new Todo { Id = 1, Title = "Test" } };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

        var result = await _service.GetAllTodosAsync();

        Assert.Equal(todos, result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTodoByIdAsync_CallsRepositoryGetById_AndReturnsResult()
    {
        var todo = new Todo { Id = 1, Title = "Test" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        var result = await _service.GetTodoByIdAsync(1);

        Assert.Equal(todo, result);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_CallsRepositoryUpdate_AndReturnsTrue_WhenExists()
    {
        var todo = new Todo { Title = "Updated" };
        _mockRepository.Setup(r => r.UpdateAsync(1, todo)).ReturnsAsync(true);

        var result = await _service.UpdateTodoAsync(1, todo);

        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(1, todo), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_CallsRepositoryUpdate_AndReturnsFalse_WhenNotFound()
    {
        var todo = new Todo { Title = "Ghost" };
        _mockRepository.Setup(r => r.UpdateAsync(9999, todo)).ReturnsAsync(false);

        var result = await _service.UpdateTodoAsync(9999, todo);

        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(9999, todo), Times.Once);
    }

    [Fact]
    public async Task DeleteTodoAsync_CallsRepositoryDelete_AndReturnsTrue_WhenDeleted()
    {
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteTodoAsync(1);

        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTodoAsync_CallsRepositoryDelete_AndReturnsFalse_WhenNotFound()
    {
        _mockRepository.Setup(r => r.DeleteAsync(9999)).ReturnsAsync(false);

        var result = await _service.DeleteTodoAsync(9999);

        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(9999), Times.Once);
    }
}