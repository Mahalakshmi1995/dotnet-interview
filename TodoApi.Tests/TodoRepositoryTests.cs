using Xunit;
using Microsoft.EntityFrameworkCore;
using TodoApi.Db;
using TodoApi.Repositories;
using TodoApi.Models;
using static TodoApi.Db.TodoDbcontext;

namespace TodoApi.Tests;

public class TodoRepositoryTests : IDisposable
{
    private readonly TodoDbContext _context;
    private readonly TodoRepository _repository;

    public TodoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        _context = new TodoDbContext(options);
        _repository = new TodoRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public void Create_MutatesTodoWithId_WhenSuccessful()
    {
        var todo = new Todo { Title = "Test", Description = "Desc", IsCompleted = false };

        _repository.Create(todo);

        Assert.True(todo.Id > 0);
        Assert.Equal("Test", todo.Title);
        Assert.False(todo.IsDeleted);
    }

    [Fact]
    public void GetAll_ReturnsEmptyList_WhenNoTodos()
    {
        var result = _repository.GetAll();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ReturnsAllNonDeletedTodos()
    {
        _repository.Create(new Todo { Title = "Todo 1", Description = "D1" });
        _repository.Create(new Todo { Title = "Todo 2", Description = "D2" });

        var result = _repository.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_ReturnsOnlyNonDeletedTodos()
    {
        var todo1 = new Todo { Title = "Todo 1", Description = "D1" };
        var todo2 = new Todo { Title = "Todo 2", Description = "D2" };
        _repository.Create(todo1);
        _repository.Create(todo2);
        _repository.Delete(todo2.Id);

        var result = _repository.GetAll();

        Assert.Single(result);
        Assert.Equal("Todo 1", result[0].Title);
    }

    [Fact]
    public void GetById_ReturnsTodo_WhenExists()
    {
        var todo = new Todo { Title = "Find Me", Description = "Desc" };
        _repository.Create(todo);

        var result = _repository.GetById(todo.Id);

        Assert.NotNull(result);
        Assert.Equal("Find Me", result.Title);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenNotFound()
    {
        var result = _repository.GetById(9999);

        Assert.Null(result);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenSoftDeleted()
    {
        var todo = new Todo { Title = "Deleted Todo", Description = "Desc" };
        _repository.Create(todo);
        _repository.Delete(todo.Id);

        var result = _repository.GetById(todo.Id);

        Assert.Null(result);
    }

    [Fact]
    public void Update_ReturnsTrue_WhenExists()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        _repository.Create(todo);

        var updated = new Todo { Title = "Updated", Description = "New Desc", IsCompleted = true };
        var result = _repository.Update(todo.Id, updated);

        Assert.True(result);
    }

    [Fact]
    public void Update_SetsUpdatedAt_WhenSuccessful()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        _repository.Create(todo);

        _repository.Update(todo.Id, new Todo { Title = "Updated", Description = "New Desc" });

        var result = _repository.GetById(todo.Id);
        Assert.NotNull(result!.UpdatedAt);
    }

    [Fact]
    public void Update_ReturnsFalse_WhenNotFound()
    {
        var result = _repository.Update(9999, new Todo { Title = "Ghost" });

        Assert.False(result);
    }

    [Fact]
    public void Update_ReturnsFalse_WhenSoftDeleted()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        _repository.Create(todo);
        _repository.Delete(todo.Id);

        var result = _repository.Update(todo.Id, new Todo { Title = "Updated" });

        Assert.False(result);
    }

    [Fact]
    public void Delete_ReturnsTrue_WhenDeleted()
    {
        var todo = new Todo { Title = "Delete Me", Description = "Desc" };
        _repository.Create(todo);

        var result = _repository.Delete(todo.Id);

        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenNotFound()
    {
        var result = _repository.Delete(9999);

        Assert.False(result);
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenAlreadyDeleted()
    {
        var todo = new Todo { Title = "Delete Me", Description = "Desc" };
        _repository.Create(todo);
        _repository.Delete(todo.Id);

        var result = _repository.Delete(todo.Id);

        Assert.False(result);
    }
}