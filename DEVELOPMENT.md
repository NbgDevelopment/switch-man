# Development Guide

This guide provides information for developers working on the Switch Man project.

## Development Environment Setup

### Prerequisites

1. **Visual Studio 2022 (17.8 or later)** or **Visual Studio Code** with:
   - C# Dev Kit extension
   - C# extension

2. **.NET 10 SDK**
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
   - Verify installation: `dotnet --version`

3. **Docker** (for containerized deployment)
   - Download from [docker.com](https://www.docker.com/)

### First-Time Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/NbgDevelopment/switch-man.git
   cd switch-man
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run --project NbgDev.SwitchMan.App
   ```

## Project Architecture

### Blazor Server

The app uses Blazor Server with:
- **Models**: Data structures (e.g., `Vlan`)
- **Services**: Business logic and data management
- **Components**: Blazor components and pages

### Dependency Injection

Services are registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<VlanService>();
```

Access services in components:
```csharp
@inject VlanService VlanService
```

## Code Organization

### Adding a New Model

1. Create class in `Models/` folder
2. Define properties with getters/setters

### Adding a New Service

1. Create class in `Services/` folder
2. Register in `Program.cs`
3. Choose appropriate lifetime:
   - `Singleton`: One instance for app lifetime
   - `Scoped`: One instance per request
   - `Transient`: New instance each time

### Adding a New Page

1. Create `.razor` file in `Components/Pages/` folder
2. Add `@page "/route"` directive at the top
3. Use `@inject` to access services
4. Add `@rendermode InteractiveServer` for interactive components

Example:
```razor
@page "/mypage"
@inject MyService MyService
@rendermode InteractiveServer

<h1>My Page</h1>

@code {
    // Component logic
}
```

## Building and Running

### Development

```bash
# Run in development mode
dotnet run --project NbgDev.SwitchMan.App

# Watch for changes (hot reload)
dotnet watch --project NbgDev.SwitchMan.App
```

### Docker

```bash
# Build Docker image
docker build -t switchman:latest .

# Run container
docker run -d -p 8080:8080 --name switchman switchman:latest

# View logs
docker logs switchman

# Stop container
docker stop switchman

# Remove container
docker rm switchman
```

## Testing

### Manual Testing Checklist

- [ ] Home page loads correctly
- [ ] Navigation menu works
- [ ] Settings page loads
- [ ] Can add VLAN with valid name and ID
- [ ] Validation prevents invalid VLAN IDs
- [ ] VLANs appear in the list
- [ ] Delete button works correctly
- [ ] Duplicate VLAN IDs are rejected
- [ ] App handles empty input gracefully
- [ ] Real-time updates work when multiple browsers connected

## Best Practices

1. **Razor Naming**:
   - Use PascalCase for component names
   - Use lowercase for parameters

2. **State Management**:
   - Use services for shared state
   - Call `StateHasChanged()` when updating UI from async operations

3. **Performance**:
   - Use `@key` directive for list items
   - Avoid unnecessary re-renders
   - Use `ShouldRender()` to optimize rendering

4. **Accessibility**:
   - Use semantic HTML elements
   - Add ARIA labels where needed
   - Ensure keyboard navigation works

## Version Control

### Commit Messages

Follow conventional commits:
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Build/tool changes

### Branch Strategy

- `main`: Stable releases
- `develop`: Integration branch
- `feature/*`: New features
- `fix/*`: Bug fixes

## Resources

- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [C# Guide](https://docs.microsoft.com/dotnet/csharp/)
- [Docker Documentation](https://docs.docker.com/)

## Getting Help

- Check existing issues on GitHub
- Consult the Blazor documentation
- Ask in team chat or discussions
- Create a new issue for bugs or feature requests
