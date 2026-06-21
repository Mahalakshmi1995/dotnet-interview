using TodoApi.Db;
using TodoApi.Models;
using static TodoApi.Db.TodoDbcontext;

namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public void Create(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);
            _context.SaveChanges();
        }

        public List<Todo> GetAll()
        {
            return _context.Todos
                .Where(t => !t.IsDeleted)
                .ToList();
        }

        public Todo? GetById(int id)
        {
            return _context.Todos
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);
        }

        public bool Update(int id, Todo todo)
        {
            var existing = _context.Todos
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (existing is null) return false;

            existing.Title = todo.Title;
            existing.Description = todo.Description;
            existing.IsCompleted = todo.IsCompleted;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var existing = _context.Todos
                .FirstOrDefault(t => t.Id == id && !t.IsDeleted);

            if (existing is null) return false;

            existing.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }
    }
}