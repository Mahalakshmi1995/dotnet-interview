using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateTodoAsync(Todo todo)
            => await _repository.CreateAsync(todo);

        public async Task<List<Todo>> GetAllTodosAsync()
            => await _repository.GetAllAsync();

        public async Task<Todo?> GetTodoByIdAsync(int id)
            => await _repository.GetByIdAsync(id);

        public async Task<bool> UpdateTodoAsync(int id, Todo todo)
            => await _repository.UpdateAsync(id, todo);

        public async Task<bool> DeleteTodoAsync(int id)
            => await _repository.DeleteAsync(id);
    }
}