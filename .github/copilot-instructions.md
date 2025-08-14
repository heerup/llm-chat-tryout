# GitHub Copilot Instructions for LLM Chat Application

## Project Overview

This is an ASP.NET Core 8.0 MVC web application that provides a chat interface for interacting with Large Language Models (LLMs) through a local Ollama server. The application focuses on simplicity, using file-based storage with clean interfaces that allow for future database migration.

### Key Features
- User authentication with role-based access (User, FeedbackReader, Admin)
- Conversation management with persistent chat history
- LLM integration via Ollama HTTP API
- Queue system for managing LLM requests
- Feedback system for rating LLM responses
- File-based storage with JSON serialization

## Architecture Patterns

### MVC Structure
- **Controllers**: Handle HTTP requests, coordinate between services, manage user sessions
- **Models**: ViewModels for data transfer between controllers and views
- **Data Layer**: Services implement interfaces, models represent domain entities
- **Views**: Razor views with minimal JavaScript, Bootstrap for styling

### Service Layer Pattern
```csharp
// All services follow this pattern with interface abstraction
public interface IExampleService
{
    Task<Entity> GetByIdAsync(string id);
    Task<Entity> CreateAsync(Entity entity);
    Task<Entity> UpdateAsync(Entity entity);
    Task DeleteAsync(string id);
}
```

### File-Based Storage
- JSON files in `Data/` directory (gitignored for runtime data)
- Each entity type has its own storage pattern:
  - Users: `Data/users.json`
  - Conversations: `Data/conversations/{id}/meta.json` and `messages.json`
  - Queue: `Data/queue/active.json`

## Code Style and Conventions

### C# Conventions
- Use file-scoped namespaces: `namespace LlmChatApp.Controllers;`
- Async/await for all I/O operations
- Dependency injection for all services
- Nullable reference types enabled
- Record types for DTOs where appropriate

### Naming Conventions
- Controllers: `{Feature}Controller` (e.g., `ChatController`)
- Services: `{Entity}Service` implementing `I{Entity}Service`
- Models: Domain entities in `Data/Models/`, ViewModels in `Models/`
- Views: Organized by controller in `Views/{Controller}/`

### Session Management
- Simple session-based authentication with 30-day cookies
- User ID stored in session: `HttpContext.Session.GetString("UserId")`
- Role-based authorization using simple string comparisons

## Domain-Specific Guidance

### LLM Integration (Ollama)
- All LLM calls are asynchronous via HTTP client
- Use conversation history for context-aware responses
- Default model: `granite3.1-moe:1b`
- Endpoint: `http://localhost:11434/api/chat`
- Handle timeouts and errors gracefully

### Conversation Management
- GUIDs for conversation IDs to ensure uniqueness
- Auto-generate conversation titles from first message
- Lazy loading for conversation history
- URL pattern: `/chat/{conversationId}`

### Message Types
- User messages: `Role = "user"`
- Assistant messages: `Role = "assistant"`
- Store timestamps and metadata for all messages

### Queue System
- Simple FIFO queue for LLM processing
- Track request status and estimated completion times
- Prevent spam by disabling input during processing

## Development Patterns

### Adding New Features
1. Create interface in `Data/Interfaces/`
2. Implement service in `Data/Services/`
3. Register in `Program.cs` dependency injection
4. Add controller action or new controller
5. Create corresponding view if needed

### Error Handling
- Use try-catch blocks for external service calls (Ollama)
- Return user-friendly error messages
- Log errors for debugging (built-in ASP.NET Core logging)

### Testing Approach
- Focus on integration testing for key user flows
- Mock external dependencies (Ollama HTTP calls)
- Test file-based storage operations

## File Organization

```
/
├── Controllers/          # MVC controllers
├── Data/
│   ├── Interfaces/      # Service contracts
│   ├── Models/          # Domain entities
│   └── Services/        # Service implementations
├── Models/              # ViewModels for MVC
├── Views/               # Razor views
├── wwwroot/            # Static files
└── docs/               # Documentation
```

## Common Development Tasks

### Adding a New Entity
1. Create model class in `Data/Models/`
2. Create service interface in `Data/Interfaces/`
3. Implement file-based service in `Data/Services/`
4. Register service in `Program.cs`
5. Add controller actions as needed

### Adding Authentication to Controller
```csharp
public IActionResult SecureAction()
{
    var userId = HttpContext.Session.GetString("UserId");
    if (string.IsNullOrEmpty(userId))
    {
        return RedirectToAction("Login", "Account");
    }
    // Continue with authenticated logic
}
```

### Working with File Storage
- Use `Path.Combine()` for file paths
- Ensure directories exist before writing files
- Use `JsonConvert.SerializeObject()` for JSON serialization
- Handle file locking and concurrent access appropriately

## LLM Service Integration

### Making LLM Calls
```csharp
var request = new
{
    model = "granite3.1-moe:1b",
    messages = conversationHistory,
    stream = false
};

var response = await _httpClient.PostAsJsonAsync(
    "http://localhost:11434/api/chat", 
    request);
```

### Context Management
- Send full conversation history with each request
- Limit conversation length to prevent token overflow
- Store LLM response metadata (model, timing, tokens)

## UI/UX Patterns

### Bootstrap Integration
- Use standard Bootstrap components and classes
- Responsive design with mobile-first approach
- Minimal custom CSS, leverage Bootstrap utilities

### JavaScript Usage
- Vanilla JavaScript only (no external libraries)
- Focus on form handling and input validation
- Disable form inputs during processing to prevent spam
- Auto-scroll chat interface to show latest messages

## Security Considerations

### Session Security
- HTTP-only cookies for session management
- 30-day session timeout
- Simple role-based authorization

### File Access
- Store user data in predictable locations
- Use GUIDs to prevent directory traversal
- Validate user ownership before file access

## Performance Guidelines

### File I/O
- Use async file operations
- Cache frequently accessed data in memory
- Implement lazy loading for large conversation histories

### LLM Performance
- Implement request queuing to manage load
- Set reasonable timeouts (30 seconds default)
- Provide user feedback during processing

## Migration Readiness

The application is designed for easy migration from file-based to database storage:
- All data access goes through service interfaces
- Services can be swapped by changing DI registration
- File storage patterns map directly to database tables
- Entity models are ready for Entity Framework

## Debugging and Troubleshooting

### Common Issues
- Ollama server not running: Check `http://localhost:11434`
- File permissions: Ensure app can read/write to `Data/` directory
- Session issues: Check session configuration in `Program.cs`
- JSON serialization: Verify model properties match JSON structure

### Logging
- Use built-in ASP.NET Core logging
- Log external service calls (Ollama) for debugging
- Log file I/O operations for troubleshooting storage issues