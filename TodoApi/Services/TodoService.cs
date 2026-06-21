
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
 
        public Todo CreateTodo(Todo todo) => _repository.Create(todo);
 
        public List<Todo> GetAllTodos() => _repository.GetAll();
 
        public Todo? GetTodoById(int id) => _repository.GetById(id);
 
        public Todo? UpdateTodo(int id, Todo todo) => _repository.Update(id, todo);
 
        public bool DeleteTodo(int id) => _repository.Delete(id);
    }
}