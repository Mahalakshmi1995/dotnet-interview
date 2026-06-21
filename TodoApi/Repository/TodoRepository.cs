using Microsoft.Data.Sqlite;
using TodoApi.Models;
 
namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly string _connectionString;
 
        public TodoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
 
        public Todo Create(Todo todo)
        {
            var createdAt = DateTime.UtcNow;
 
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
 
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                VALUES (@Title, @Description, @IsCompleted, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("o"));
 
            todo.Id = Convert.ToInt32(command.ExecuteScalar());
            todo.CreatedAt = createdAt;
            return todo;
        }
 
        public List<Todo> GetAll()
        {
            var todos = new List<Todo>();
 
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
 
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos";
 
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                todos.Add(MapTodo(reader));
            }
 
            return todos;
        }
 
        public Todo? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
 
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
 
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapTodo(reader) : null;
        }
 
        public Todo? Update(int id, Todo todo)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
 
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Todos
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Title", todo.Title);
            command.Parameters.AddWithValue("@Description", (object?)todo.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsCompleted", todo.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Id", id);
 
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0) return null;
 
            todo.Id = id;
            return todo;
        }
 
        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
 
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
 
            return command.ExecuteNonQuery() > 0;
        }
 
        private static Todo MapTodo(SqliteDataReader reader) => new Todo
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
            IsCompleted = reader.GetInt32(reader.GetOrdinal("IsCompleted")) == 1,
            CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
        };
    }
}