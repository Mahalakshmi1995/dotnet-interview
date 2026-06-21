using Xunit;
using TodoApi.Repositories;
using TodoApi.Models;
using Microsoft.Data.Sqlite;
 
namespace TodoApi.Tests;
 
public class TodoRepositoryTests : IDisposable
{
    private readonly string _connectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _keepAlive;
    private readonly TodoRepository _repository;
 
    public TodoRepositoryTests()
    {
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
 
        _repository = new TodoRepository(_connectionString);
    }
 
    public void Dispose()
    {
        var command = _keepAlive.CreateCommand();
        command.CommandText = "DROP TABLE IF EXISTS Todos";
        command.ExecuteNonQuery();
        _keepAlive.Dispose();
    }
 
    [Fact]
    public void Create_ReturnsCreatedTodoWithId()
    {
        var todo = new Todo { Title = "Test", Description = "Desc", IsCompleted = false };
 
        var result = _repository.Create(todo);
 
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test", result.Title);
    }
 
    [Fact]
    public void GetAll_ReturnsEmptyList_WhenNoTodos()
    {
        var result = _repository.GetAll();
 
        Assert.NotNull(result);
        Assert.Empty(result);
    }
 
    [Fact]
    public void GetAll_ReturnsAllTodos()
    {
        _repository.Create(new Todo { Title = "Todo 1", Description = "D1" });
        _repository.Create(new Todo { Title = "Todo 2", Description = "D2" });
 
        var result = _repository.GetAll();
 
        Assert.Equal(2, result.Count);
    }
 
    [Fact]
    public void GetById_ReturnsTodo_WhenExists()
    {
        var created = _repository.Create(new Todo { Title = "Find Me", Description = "Desc" });
 
        var result = _repository.GetById(created.Id);
 
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
    public void Update_ReturnsUpdatedTodo_WhenExists()
    {
        var created = _repository.Create(new Todo { Title = "Original", Description = "Desc" });
        var updated = new Todo { Title = "Updated", Description = "New Desc", IsCompleted = true };
 
        var result = _repository.Update(created.Id, updated);
 
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Title);
        Assert.True(result.IsCompleted);
    }
 
    [Fact]
    public void Update_ReturnsNull_WhenNotFound()
    {
        var result = _repository.Update(9999, new Todo { Title = "Ghost" });
 
        Assert.Null(result);
    }
 
    [Fact]
    public void Delete_ReturnsTrue_WhenDeleted()
    {
        var created = _repository.Create(new Todo { Title = "Delete Me", Description = "Desc" });
 
        var result = _repository.Delete(created.Id);
 
        Assert.True(result);
    }
 
    [Fact]
    public void Delete_ReturnsFalse_WhenNotFound()
    {
        var result = _repository.Delete(9999);
 
        Assert.False(result);
    }
}