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
3. Add `@rendermode InteractiveServer` for interactive components
4. Create a corresponding `.razor.cs` code-behind file (see Code-Behind Pattern below)

Example `.razor` file:
```razor
@page "/mypage"
@rendermode InteractiveServer

<h1>My Page</h1>
```

Example `.razor.cs` code-behind file:
```csharp
using Microsoft.AspNetCore.Components;

namespace NbgDev.SwitchMan.App.Components.Pages;

public partial class MyPage
{
    [Inject]
    private MyService MyService { get; set; } = null!;

    // Component logic, properties, and methods
}
```

### Code-Behind Pattern

**All Razor components MUST use the code-behind pattern** to separate markup from logic:

1. **Create the code-behind file**:
   - Name: `ComponentName.razor.cs` (e.g., `Home.razor.cs`)
   - Location: Same directory as the `.razor` file
   - Namespace: Match the component's location (e.g., `NbgDev.SwitchMan.App.Components.Pages`)
   - Class: `public partial class ComponentName`

2. **Structure of code-behind files**:
   ```csharp
   using Microsoft.AspNetCore.Components;
   using NbgDev.SwitchMan.App.Models;
   using NbgDev.SwitchMan.App.Services;

   namespace NbgDev.SwitchMan.App.Components.Pages;

   public partial class ComponentName
   {
       // Dependency injection using [Inject] attribute
       [Inject]
       private MyService MyService { get; set; } = null!;

       // Parameters using [Parameter] attribute
       [Parameter]
       public string Title { get; set; } = string.Empty;

       // Component state (private fields)
       private string myField = string.Empty;

       // Component methods
       private void MyMethod()
       {
           // Logic here
       }

       // Lifecycle methods
       protected override void OnInitialized()
       {
           // Initialization logic
       }
   }
   ```

3. **Razor file should contain only**:
   - Directives (`@page`, `@rendermode`, etc.)
   - HTML markup
   - Razor syntax for data binding and rendering

4. **DO NOT use `@code` blocks** in `.razor` files
   - All C# code belongs in the `.razor.cs` code-behind file
   - This improves code organization, testability, and maintainability

5. **DO NOT use `@inject` in `.razor` files**
   - Use `[Inject]` attribute on properties in the code-behind file instead
   - This keeps all dependencies clearly defined in one place

6. **DO NOT use `@using` directives in individual `.razor` files**
   - Common usings are defined in `Components/_Imports.razor`
   - Component-specific usings go in the `.razor.cs` code-behind file

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
- [ ] **Mobile Testing**:
  - [ ] UI is usable on smartphone screens (375px - 428px wide)
  - [ ] All buttons are easily tappable (minimum 44x44px touch targets)
  - [ ] Text is readable without zooming
  - [ ] Forms are easy to fill out on mobile
  - [ ] Modals display correctly on small screens
  - [ ] No horizontal scrolling on mobile viewports

## Best Practices

1. **Code-Behind Pattern** (REQUIRED):
   - **ALWAYS** create a `.razor.cs` code-behind file for components with logic
   - **NEVER** use `@code` blocks in `.razor` files
   - **NEVER** use `@inject` in `.razor` files (use `[Inject]` in code-behind instead)
   - Keep markup in `.razor`, logic in `.razor.cs`
   - See "Code-Behind Pattern" section above for details

2. **Razor Naming**:
   - Use PascalCase for component names
   - Use lowercase for parameters

3. **State Management**:
   - Use services for shared state
   - Call `StateHasChanged()` when updating UI from async operations

4. **Performance**:
   - Use `@key` directive for list items
   - Avoid unnecessary re-renders
   - Use `ShouldRender()` to optimize rendering

5. **Accessibility**:
   - Use semantic HTML elements
   - Add ARIA labels where needed
   - Ensure keyboard navigation works

6. **Mobile-First Design** (REQUIRED):
   - **ALWAYS** design UI components for mobile devices first
   - Ensure all interactive elements meet minimum touch target size (44x44px)
   - Test on mobile viewports (375px - 428px wide) during development
   - Use responsive CSS with mobile-first media queries
   - Avoid horizontal scrolling on mobile devices
   - Use larger form controls on mobile (form-control-lg)
   - Ensure text is readable without zooming (minimum 16px font size for body text)

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
