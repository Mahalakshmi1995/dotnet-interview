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
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TodoDbContext(options);
        _repository = new TodoRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Create_MutatesTodoWithId_WhenSuccessful()
    {
        var todo = new Todo { Title = "Test", Description = "Desc", IsCompleted = false };

        await _repository.CreateAsync(todo);

        Assert.True(todo.Id > 0);
        Assert.Equal("Test", todo.Title);
        Assert.False(todo.IsDeleted);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoTodos()
    {
        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllNonDeletedTodos()
    {
        await _repository.CreateAsync(new Todo { Title = "Todo 1", Description = "D1" });
        await _repository.CreateAsync(new Todo { Title = "Todo 2", Description = "D2" });

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsOnlyNonDeletedTodos()
    {
        var todo1 = new Todo { Title = "Todo 1", Description = "D1" };
        var todo2 = new Todo { Title = "Todo 2", Description = "D2" };
        await _repository.CreateAsync(todo1);
        await _repository.CreateAsync(todo2);
        await _repository.DeleteAsync(todo2.Id);

        var result = await _repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Todo 1", result[0].Title);
    }

    [Fact]
    public async Task GetById_ReturnsTodo_WhenExists()
    {
        var todo = new Todo { Title = "Find Me", Description = "Desc" };
        await _repository.CreateAsync(todo);

        var result = await _repository.GetByIdAsync(todo.Id);

        Assert.NotNull(result);
        Assert.Equal("Find Me", result.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(9999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenSoftDeleted()
    {
        var todo = new Todo { Title = "Deleted Todo", Description = "Desc" };
        await _repository.CreateAsync(todo);
        await _repository.DeleteAsync(todo.Id);

        var result = await _repository.GetByIdAsync(todo.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_ReturnsTrue_WhenExists()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        await _repository.CreateAsync(todo);

        var updated = new Todo { Title = "Updated", Description = "New Desc", IsCompleted = true };
        var result = await _repository.UpdateAsync(todo.Id, updated);

        Assert.True(result);
    }

    [Fact]
    public async Task Update_SetsUpdatedAt_WhenSuccessful()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        await _repository.CreateAsync(todo);

        await _repository.UpdateAsync(todo.Id, new Todo { Title = "Updated", Description = "New Desc" });

        var result = await _repository.GetByIdAsync(todo.Id);
        Assert.NotNull(result!.UpdatedAt);
    }

    [Fact]
    public async Task Update_ReturnsFalse_WhenNotFound()
    {
        var result = await _repository.UpdateAsync(9999, new Todo { Title = "Ghost" });

        Assert.False(result);
    }

    [Fact]
    public async Task Update_ReturnsFalse_WhenSoftDeleted()
    {
        var todo = new Todo { Title = "Original", Description = "Desc" };
        await _repository.CreateAsync(todo);
        await _repository.DeleteAsync(todo.Id);

        var result = await _repository.UpdateAsync(todo.Id, new Todo { Title = "Updated" });

        Assert.False(result);
    }

    [Fact]
    public async Task Delete_ReturnsTrue_WhenDeleted()
    {
        var todo = new Todo { Title = "Delete Me", Description = "Desc" };
        await _repository.CreateAsync(todo);

        var result = await _repository.DeleteAsync(todo.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenNotFound()
    {
        var result = await _repository.DeleteAsync(9999);

        Assert.False(result);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenAlreadyDeleted()
    {
        var todo = new Todo { Title = "Delete Me", Description = "Desc" };
        await _repository.CreateAsync(todo);
        await _repository.DeleteAsync(todo.Id);

        var result = await _repository.DeleteAsync(todo.Id);

        Assert.False(result);
    }
}