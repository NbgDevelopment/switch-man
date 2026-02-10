# Development Guide

This guide provides information for developers working on the Switch Man project.

## Development Environment Setup

### Prerequisites

1. **Visual Studio 2022 (17.8 or later)** with:
   - .NET desktop development workload
   
   OR
   
   **Visual Studio Code** with:
   - C# Dev Kit extension

2. **.NET 10 SDK**
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
   - Verify installation: `dotnet --version`

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

## Project Architecture

### MVVM Pattern (Simplified)

The app uses a simplified approach:
- **Models**: Data structures (e.g., `Vlan`)
- **Services**: Business logic and data management
- **Windows**: WPF windows with code-behind for interactions

### Dependency Management

Services are instantiated directly in windows for simplicity:

```csharp
private readonly VlanService _vlanService = new VlanService();
```

## Code Organization

### Adding a New Model

1. Create class in `Models/` folder
2. Define properties with getters/setters
3. Consider implementing `INotifyPropertyChanged` for data binding

### Adding a New Service

1. Create class in `Services/` folder
2. Implement business logic methods
3. Use `ObservableCollection<T>` for data that binds to UI

### Adding a New Window

1. Create XAML and code-behind files in `Windows/` folder
2. Register window as needed in application
3. Use `ShowDialog()` for modal windows, `Show()` for modeless

Example XAML structure:
```xml
<Window x:Class="NbgDev.SwitchMan.App.Windows.MyWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="My Window" Height="450" Width="800">
    <Grid>
        <!-- Content here -->
    </Grid>
</Window>
```

## Building and Running

### Debug Build

```bash
dotnet build
```

### Release Build

```bash
dotnet build -c Release
```

### Running the Application

```bash
dotnet run --project NbgDev.SwitchMan.App
```

Or in Visual Studio: Press F5

## Testing

### Manual Testing Checklist

- [ ] Main window loads correctly
- [ ] Settings button opens settings window
- [ ] Can add VLAN with valid name and ID
- [ ] Validation prevents invalid VLAN IDs
- [ ] VLANs appear in the list
- [ ] Delete selected works correctly
- [ ] Duplicate VLAN IDs are rejected
- [ ] App handles empty input gracefully

## Best Practices

1. **XAML Naming**:
   - Use `x:Name` for controls accessed in code-behind
   - Use PascalCase for names

2. **Event Handlers**:
   - Use descriptive names: `OnSomethingClicked`
   - Keep handlers focused and simple

3. **Data Binding**:
   - Bind to ObservableCollections for automatic UI updates
   - Use StringFormat in bindings when needed

4. **Resource Management**:
   - Dispose of resources when done
   - Unsubscribe from events

5. **Accessibility**:
   - Use Labels for form fields
   - Set meaningful window titles

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

Example:
```
feat: add VLAN validation
fix: resolve duplicate ID issue
docs: update development guide
```

### Branch Strategy

- `main`: Stable releases
- `develop`: Integration branch
- `feature/*`: New features
- `fix/*`: Bug fixes

## Resources

- [WPF Documentation](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [XAML Documentation](https://docs.microsoft.com/dotnet/desktop/wpf/xaml/)
- [C# Guide](https://docs.microsoft.com/dotnet/csharp/)
- [GitHub Copilot Instructions](.github/copilot-instructions.md)

## Getting Help

- Check existing issues on GitHub
- Consult the WPF documentation
- Ask in team chat or discussions
- Create a new issue for bugs or feature requests
