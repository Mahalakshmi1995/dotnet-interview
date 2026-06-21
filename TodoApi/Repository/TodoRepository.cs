using Microsoft.EntityFrameworkCore;
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

        public async Task CreateAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            return await _context.Todos
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<bool> UpdateAsync(int id, Todo todo)
        {
            var existing = await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (existing is null) return false;

            existing.Title = todo.Title;
            existing.Description = todo.Description;
            existing.IsCompleted = todo.IsCompleted;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (existing is null) return false;

            existing.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}