# LLM Chat Application - Architecture Documentation

## System Overview

The LLM Chat Application is a self-contained ASP.NET Core MVC application that provides a simple, efficient chat interface for interacting with Large Language Models (LLMs) through a local Ollama server. The application emphasizes simplicity, minimal dependencies, and straightforward deployment.

## Architecture Principles

### Simplicity First
- **Minimal Error Handling**: Leverage ASP.NET Core defaults and built-in error handling
- **Vanilla JavaScript**: Use native JavaScript only when absolutely necessary, avoid external libraries
- **File-based Storage**: Simple JSON files for data persistence, easily migrated to database later
- **Convention over Configuration**: Follow ASP.NET Core conventions to minimize setup

### Core Design Patterns

#### MVC Pattern
- **Controllers**: Handle HTTP requests and coordinate between services and views
- **Models**: Simple POCOs for data transfer and view models
- **Views**: Razor pages with minimal client-side logic

#### Repository Pattern (Simplified)
- **Interfaces**: Abstract data access to enable future database migration
- **File Services**: JSON-based implementations for immediate development needs
- **In-Memory Caching**: Simple caching layer for frequently accessed data

## System Components

### 1. Presentation Layer
```
Controllers/
├── HomeController.cs          # Landing page and navigation
├── ChatController.cs          # Chat interface and message handling
├── AuthController.cs          # User authentication
└── AdminController.cs         # Administrative functions
```

### 2. Business Logic Layer
```
Services/
├── IConversationService.cs     # Conversation management
├── ILlmService.cs             # LLM integration (Ollama with chat endpoint)
├── IUserService.cs            # User management  
├── IMessageService.cs         # Message persistence
├── IAuthService.cs            # Authentication handling
└── IQueueService.cs           # Request queuing
```

#### LLM Integration Design
- **Chat Endpoint**: Uses Ollama's `/api/chat` endpoint for conversation context
- **Conversation History**: Full message history passed to LLM for context awareness
- **Configurable Model**: Default model `granite3.1-moe:1b` configurable via appsettings
- **Async Processing**: Queue-based processing for concurrent user requests

### 3. Data Access Layer
```
Data/
├── Interfaces/
│   ├── IUserRepository.cs
│   ├── IConversationRepository.cs
│   ├── IMessageRepository.cs
│   ├── IFeedbackRepository.cs
│   └── IQueueRepository.cs
├── Models/
│   ├── User.cs
│   ├── Conversation.cs
│   ├── Message.cs
│   ├── Feedback.cs
│   └── QueueItem.cs
└── Services/
    ├── FileUserRepository.cs
    ├── FileConversationRepository.cs
    ├── FileMessageRepository.cs
    ├── FileFeedbackRepository.cs
    └── FileQueueRepository.cs
```

### 4. External Integration
```
Integration/
├── OllamaClient.cs           # HTTP client for Ollama API
└── Models/
    ├── OllamaRequest.cs
    └── OllamaResponse.cs
```

## Data Flow Architecture

### 1. User Chat Request Flow
```
User Input → ChatController → ChatService → LlmService → Ollama → Response → View
```

### 2. Queue Management Flow
```
Request → QueueService → ProcessingEngine → LlmService → Completion → Notification
```

### 3. Feedback Collection Flow
```
User Feedback → FeedbackController → FeedbackService → Repository → Storage
```

## Security Architecture

### Authentication
- **File-based User Store**: Simple JSON file for user credentials
- **Persistent Session Authentication**: 30-day session cookies for user convenience
- **Role-based Authorization**: User, Admin roles

### Data Protection
- **Password Hashing**: BCrypt for password storage
- **Session Security**: Secure cookies with appropriate flags
- **Input Validation**: Model validation and sanitization

## Storage Architecture

### File-based Storage Structure
```
data/
├── users.json               # User accounts and roles
├── conversations/
│   ├── index.json          # Conversation metadata
│   └── {id}/
│       ├── meta.json       # Conversation details
│       └── messages.json   # Message history
├── feedback/
│   └── {conversation-id}.json
└── queue/
    └── active.json
```

### Data Models
- **User**: Authentication and role information
- **Conversation**: Chat session metadata with unique URLs
- **Message**: Individual chat messages with timestamps
- **Feedback**: User ratings and comments on responses
- **QueueItem**: Pending requests with priority and status

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **JSON Handling**: Newtonsoft.Json
- **HTTP Client**: Microsoft.Extensions.Http
- **Authentication**: ASP.NET Core Identity (simplified)

### Frontend
- **Views**: Razor Pages with minimal JavaScript
- **Styling**: Bootstrap (included by default)
- **JavaScript**: Vanilla JS only, used sparingly

### External Services
- **LLM Provider**: Ollama (local installation)
- **Models**: Configurable LLM models through Ollama

## Scalability Considerations

### Current Design (File-based)
- **Development Focus**: Quick setup and iteration
- **User Capacity**: Small to medium user base (< 100 concurrent)
- **Storage**: Local file system with simple JSON format

### Future Migration Path
- **Database**: Entity Framework Core ready interfaces
- **Caching**: Redis for session and conversation caching
- **Load Balancing**: Stateless design enables horizontal scaling
- **Message Queue**: Background service for LLM processing

## Deployment Architecture

### Development Environment
- **Local Ollama**: Development LLM server on localhost
- **File Storage**: Local data directory
- **IIS Express**: Development web server

### Production Environment
- **Containerization**: Docker support for easy deployment
- **Reverse Proxy**: Nginx or IIS for static file serving
- **Data Persistence**: Mounted volumes for data directory
- **Monitoring**: ASP.NET Core built-in logging and health checks

## Performance Considerations

### Response Time Optimization
- **Async Operations**: All LLM calls are asynchronous
- **Streaming Responses**: Real-time message display (if supported by Ollama)
- **Caching**: In-memory conversation caching
- **Pagination**: Message history pagination for large conversations

### Resource Management
- **Connection Pooling**: HTTP client connection reuse
- **Memory Management**: Dispose pattern for large objects
- **File I/O**: Async file operations to prevent blocking
- **Queue Processing**: Background service for non-critical operations

## Error Handling Strategy

### Principle: Lean on Framework
- **Global Exception Handler**: ASP.NET Core default error pages
- **Model Validation**: Data annotations for input validation
- **Service Errors**: Simple try-catch with logging, graceful degradation
- **LLM Failures**: Retry logic with user notification

### User Experience
- **Graceful Degradation**: Application continues working if LLM is unavailable
- **User Feedback**: Clear error messages without technical details
- **Recovery Options**: Retry mechanisms for failed operations