using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Task CreateAsync(Todo todo);
        Task<List<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Todo todo);
        Task<bool> DeleteAsync(int id);
    }
}