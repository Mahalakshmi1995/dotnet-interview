using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task CreateTodoAsync(Todo todo);
        Task<List<Todo>> GetAllTodosAsync();
        Task<Todo?> GetTodoByIdAsync(int id);
        Task<bool> UpdateTodoAsync(int id, Todo todo);
        Task<bool> DeleteTodoAsync(int id);
    }
}