# Solution Documentation

**Candidate Name:** [Mahalakshmi G]  
**Completion Date:** [22/06/2026]

---

## Problems Identified

_Describe the issues you found in the original implementation. Consider aspects like:_
- Architecture and design patterns
- Code quality and maintainability
- Security vulnerabilities
- Performance concerns
- Testing gaps

No separation of concerns — the original TodoController instantiated new TodoService() inside every action method, bypassing dependency injection entirely and making the code untestable.
No repository layer — TodoService contained both business logic and raw SQL queries mixed together, violating the Single Responsibility Principle.
No interface for TodoService — the controller depended on the concrete class directly, making it impossible to mock for unit testing.
Wrong HTTP verbs — all endpoints used [HttpPost], including reads and deletes, violating REST conventions.
Verb names in routes — routes like api/createTodo, api/getTodo embedded actions in URLs instead of using noun-based REST routes.
SQL injection — all SQL queries used string interpolation directly with user input (e.g., $"INSERT INTO Todos ... VALUES ('{todo.Title}'") making the API vulnerable to SQL injection attacks.
Raw exception messages exposed — return BadRequest(ex.Message) leaked internal stack details to clients.
Hardcoded connection string — private string _connectionString = "Data Source=todos.db" was hardcoded in TodoService instead of being read from configuration.
Repeated code — every repository method duplicated the same connection setup boilerplate.
No input validation — no constraints on Title length or required fields, allowing empty or oversized data to reach the database.

---

## Architectural Decisions

_Explain the architecture you chose and why. Consider:_
- Design patterns applied
- Project structure changes
- Technology choices
- Separation of concerns

Architecture and Design Patterns
Layered Architecture

Introduced a clean three-layer architecture with strict separation of concerns:

Controller → ITodoService → ITodoRepository → Database


TodoController — handles HTTP only (routing, request parsing, response formatting)
TodoService — handles business logic, delegates data access to the repository
TodoRepository — handles all database operations
Interfaces (ITodoService, ITodoRepository) — enable dependency injection and mocking
Repository Pattern — extracted all database access into TodoRepository, so if the database changes (e.g., SQLite → SQL Server), only the repository changes and nothing else is affected.

Dependency Injection — all dependencies are injected via constructors and registered in Program.cs. No new keyword for services anywhere in the codebase.

Command Query Separation (CQS) — read methods return data (GetAll, GetById), write methods return only success/failure (void Create, bool Update, bool Delete). No method both modifies state and returns data.

DRY (Don't Repeat Yourself) — extracted repeated connection setup into CreateOpenConnection() helper and row mapping into MapTodo() helper in the repository.
Created Consistent Api response format
{
  "success": true,
  "data": { ... },
  "message": null
}
Introduced soft delete - Replaced hard deletes with soft deletes using an IsDeleted flag. Records are never permanently removed — they are marked as deleted and excluded from queries. This preserves audit history and prevents accidental data loss.
Added UpdatedAt field set automatically on every update, providing a full audit trail alongside CreatedAt.
Migrated from raw Microsoft.Data.Sqlite queries to EF Core:



---

## Trade-offs

_Discuss compromises you made and the reasoning behind them. Consider:_
- What did you prioritize?
- What did you defer or simplify?
- What alternatives did you consider?
Correctness and security first — SQL injection fix was the highest priority as it was a critical vulnerability.
Testability — every layer is independently testable via interfaces and dependency injection.
Clean architecture — chose to add the repository layer even though it added more files, because it correctly separates concerns and reflects production-grade code.
Pagination — GetAllTodos returns all records. For large datasets, page and pageSize query parameters would be needed.
Authentication/Authorization — no JWT or API key authentication implemented. In production this would be required.

---

## How to Run

### Prerequisites
[List required software, versions, etc.]

### Build
```bash
dotnet build
# Add your build commands
```

### Run
```bash
# Add your run commands
dotnet run --project TodoApi
```

### Test
```bash
# Add your test commands
dotnet test
```

---

## API Documentation



### Endpoints

#### Create TODO
```
Method: POST
URL: api/todos
Request Body:
{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread",
  "isCompleted": false
}
Response (201 Created):
{
  "success": true,
  "data": {
    "id": 1,
    "title": "Buy groceries",
    "description": "Milk, eggs, bread",
    "isCompleted": false,
    "isDeleted": false,
    "createdAt": "2026-06-22T10:00:00Z",
    "updatedAt": null
  },
  "message": null
}
```

#### Get All TODO(s)
```
Method: GET
URL: api/todos
Response (200 OK):
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Buy groceries",
      "description": "Milk, eggs, bread",
      "isCompleted": false,
      "isDeleted": false,
      "createdAt": "2026-06-22T10:00:00Z",
      "updatedAt": null
    }
  ],
  "message": null
}
```

#### Get TODO(s) by ID
```
Method: GET
URL: api/todos/{id}
Response (200 OK):
{
  "success": true,
  "data": {
    "id": 1,
    "title": "Buy groceries",
    "description": "Milk, eggs, bread",
    "isCompleted": false,
    "isDeleted": false,
    "createdAt": "2026-06-22T10:00:00Z",
    "updatedAt": null
  },
  "message": null
}
Response (404 Not Found):
{
  "success": false,
  "data": null,
  "message": "Todo with id 1 not found"
}
```

#### Update TODO
```
Method: PUT
URL: api/todos/{id}
Request Body:
{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread and butter",
  "isCompleted": true
}
Response (200 OK):
{
  "success": true,
  "data": "Todo updated successfully",
  "message": null
}
Response (404 Not Found):
{
  "success": false,
  "data": null,
  "message": "Todo with id 1 not found"
}
```

#### Delete TODO
```
Method: DELETE
URL: api/todos/{id}
Response (200 OK):
{
  "success": true,
  "data": "Todo deleted successfully",
  "message": null
}
Response (404 Not Found):
{
  "success": false,
  "data": null,
  "message": "Todo with id 1 not found"
}
```

---

## Future Improvements

_What would you do if you had more time? Consider:_
- Additional features
- Performance optimizations
- Enhanced testing
- Better documentation
- Deployment considerations

Authentication — add JWT bearer token authentication to protect all endpoints
Pagination — add page and pageSize query parameters to GET api/todos
Integration tests — add end-to-end tests that spin up the full API and test HTTP responses
Load testing — use tools like k6 or NBomber to test performance under concurrent requests
Test coverage reporting — configure coverlet to enforce minimum coverage thresholds in CI
Docker — containerize the API with a Dockerfile for consistent deployments
CI/CD pipeline — add GitHub Actions to run build, test, and lint on every pull request
