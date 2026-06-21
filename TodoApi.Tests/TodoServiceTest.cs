using Xunit;
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
    public void CreateTodo_CallsRepositoryCreate()
    {
        var todo = new Todo { Title = "Test", Description = "Desc" };

        _service.CreateTodo(todo);

        _mockRepository.Verify(r => r.Create(todo), Times.Once);
    }

    [Fact]
    public void GetAllTodos_CallsRepositoryGetAll_AndReturnsResult()
    {
        var todos = new List<Todo> { new Todo { Id = 1, Title = "Test" } };
        _mockRepository.Setup(r => r.GetAll()).Returns(todos);

        var result = _service.GetAllTodos();

        Assert.Equal(todos, result);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetTodoById_CallsRepositoryGetById_AndReturnsResult()
    {
        var todo = new Todo { Id = 1, Title = "Test" };
        _mockRepository.Setup(r => r.GetById(1)).Returns(todo);

        var result = _service.GetTodoById(1);

        Assert.Equal(todo, result);
        _mockRepository.Verify(r => r.GetById(1), Times.Once);
    }

    [Fact]
    public void UpdateTodo_CallsRepositoryUpdate_AndReturnsTrue_WhenExists()
    {
        var todo = new Todo { Title = "Updated" };
        _mockRepository.Setup(r => r.Update(1, todo)).Returns(true);

        var result = _service.UpdateTodo(1, todo);

        Assert.True(result);
        _mockRepository.Verify(r => r.Update(1, todo), Times.Once);
    }

    [Fact]
    public void UpdateTodo_CallsRepositoryUpdate_AndReturnsFalse_WhenNotFound()
    {
        var todo = new Todo { Title = "Ghost" };
        _mockRepository.Setup(r => r.Update(9999, todo)).Returns(false);

        var result = _service.UpdateTodo(9999, todo);

        Assert.False(result);
        _mockRepository.Verify(r => r.Update(9999, todo), Times.Once);
    }

    [Fact]
    public void DeleteTodo_CallsRepositoryDelete_AndReturnsTrue_WhenDeleted()
    {
        _mockRepository.Setup(r => r.Delete(1)).Returns(true);

        var result = _service.DeleteTodo(1);

        Assert.True(result);
        _mockRepository.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteTodo_CallsRepositoryDelete_AndReturnsFalse_WhenNotFound()
    {
        _mockRepository.Setup(r => r.Delete(9999)).Returns(false);

        var result = _service.DeleteTodo(9999);

        Assert.False(result);
        _mockRepository.Verify(r => r.Delete(9999), Times.Once);
    }
}