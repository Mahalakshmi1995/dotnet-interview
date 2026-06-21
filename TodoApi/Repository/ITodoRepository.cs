using TodoApi.Models;
 
namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Todo Create(Todo todo);
        List<Todo> GetAll();
        Todo? GetById(int id);
        Todo? Update(int id, Todo todo);
        bool Delete(int id);
    }
}