using TodoApi.Models;
namespace TodoApi.Services
{
    public interface ITodoService
    {
        void CreateTodo(Todo todo);
        List<Todo> GetAllTodos();
        Todo? GetTodoById(int id);
        bool UpdateTodo(int id, Todo todo);
        bool DeleteTodo(int id);
    }
}