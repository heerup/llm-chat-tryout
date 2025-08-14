# LLM Chat Tryout Project

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Project Overview
LLM Chat Tryout is a simple ASP.NET Core MVC application for running an LLM chat system on on-premises infrastructure. The app uses a local Ollama server for LLM functionality and local files for storage (chat sessions, queue, user logins).

## Working Effectively

### Prerequisites and Setup
- .NET 8 SDK is available at `/usr/bin/dotnet` (version 8.0.118)
- Repository currently contains only documentation - no code has been implemented yet
- When implementing the application, use the ASP.NET Core MVC template

### Initial Project Creation (if needed)
If the project structure doesn't exist yet:
```bash
dotnet new mvc -n LlmChatTryout
```
This takes ~4 seconds to complete.

### Build Commands
- **NEVER CANCEL builds** - they complete quickly but set timeouts appropriately
- Initial build: `dotnet build` - takes ~9 seconds. Set timeout to 30+ seconds.
- Release build: `dotnet publish -c Release` - takes ~2 seconds. Set timeout to 15+ seconds.
- Restore packages: `dotnet restore` - takes ~2 seconds. Set timeout to 15+ seconds.

### Test Commands  
- Run tests: `dotnet test` - takes ~1 second. Set timeout to 15+ seconds.
- Currently no tests exist in the repository

### Development Commands
- Run development server: `dotnet run` 
  - Application starts on http://localhost:5xxx (port varies)
  - Takes ~2 seconds to start. Set timeout to 15+ seconds.
  - Press Ctrl+C to stop the server
- Watch mode (auto-restart on changes): `dotnet watch run`

### Code Quality Commands
- Format code: `dotnet format` - takes ~10 seconds. Set timeout to 30+ seconds.
- Check formatting: `dotnet format --verify-no-changes` - takes ~10 seconds. Set timeout to 30+ seconds.
- ALWAYS run `dotnet format` before committing code changes

### Package Management
- Add package: `dotnet add package <PackageName>`
- Remove package: `dotnet remove package <PackageName>`
- List packages: `dotnet list package`

## Validation Requirements

### Manual Testing Scenarios
After making changes to the application, ALWAYS test these scenarios:
1. **Application Startup**: Run `dotnet run` and verify the application starts without errors
2. **Basic Navigation**: If implemented, test that the main pages load correctly
3. **LLM Integration**: If implemented, test that chat functionality works with local Ollama server
4. **File Storage**: If implemented, verify that chat sessions are properly saved to local files

### Pre-commit Validation
ALWAYS run these commands before committing:
1. `dotnet build` - ensure code compiles
2. `dotnet test` - ensure all tests pass (when tests exist)
3. `dotnet format` - format code according to standards

## External Dependencies

### Ollama Server
- The application requires a local Ollama server for LLM functionality
- Ollama is NOT currently installed in this environment
- Installation command: `curl -fsSL https://ollama.com/install.sh | sh` (may fail due to network restrictions)
- When implementing LLM features, include fallback behavior when Ollama is unavailable

### Development Environment
- Ubuntu 24.04 Linux environment
- .NET 8.0.118 SDK installed
- ASP.NET Core 8.0.18 runtime installed
- Global tool: nbgv (3.7.115) for version management

## Repository Structure

### Current State
```
/
├── .gitignore          # .NET-specific gitignore
├── README.md          # Project documentation
└── .github/
    └── copilot-instructions.md  # This file
```

### Expected Structure (when implemented)
```
/
├── Controllers/       # MVC controllers
├── Models/           # Data models
├── Views/            # Razor views
├── wwwroot/          # Static web assets
├── Program.cs        # Application entry point
├── appsettings.json  # Configuration
├── {ProjectName}.csproj  # Project file
└── [additional folders as needed]
```

## Common Issues and Solutions

### Build Issues
- If `dotnet build` fails, check that all required packages are restored with `dotnet restore`
- If package restore fails, delete `bin/` and `obj/` folders and retry

### Runtime Issues
- Warning about "No XML encryptor configured" is normal in development
- Application may fail to start if ports are already in use - check with `netstat -tulpn`

### Ollama Integration
- Test Ollama availability before making LLM calls
- Implement graceful fallback when Ollama server is unavailable
- Document any specific Ollama models required

## Time Expectations
- Project creation: ~4 seconds
- Build operations: ~9 seconds
- Test execution: ~1 second (when tests exist)
- Code formatting: ~10 seconds
- **NEVER CANCEL** any of these operations - they complete quickly

## Development Workflow
1. Make code changes
2. Run `dotnet build` to verify compilation
3. Run `dotnet run` to test functionality
4. Perform manual validation scenarios
5. Run `dotnet format` to ensure code style
6. Run `dotnet test` (when tests exist)
7. Commit changes

Remember: This is a self-contained application designed for on-premises deployment with local file storage and Ollama integration.