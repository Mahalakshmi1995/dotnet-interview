using TodoApi.Models;
 
namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        void Create(Todo todo);
        List<Todo> GetAll();
        Todo? GetById(int id);
        bool Update(int id, Todo todo);
        bool Delete(int id);
    }
}