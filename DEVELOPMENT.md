# Development Guide

This guide provides information for developers working on the Switch Man project.

## Development Environment Setup

### Prerequisites

1. **Visual Studio 2022 (17.8 or later)** with:
   - .NET desktop development workload
   - .NET Multi-platform App UI development workload
   - Android SDK (for Android development)
   
   OR
   
   **Visual Studio Code** with:
   - C# Dev Kit extension
   - .NET MAUI extension

2. **.NET 10 SDK**
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
   - Verify installation: `dotnet --version`

3. **MAUI Workload**
   ```bash
   dotnet workload install maui
   ```

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

### MVVM-Light Pattern

The app uses a simplified MVVM approach:
- **Models**: Data structures (e.g., `Vlan`)
- **Services**: Business logic and data management
- **Pages**: Views with code-behind for simple interactions

### Dependency Injection

Services are registered in `MauiProgram.cs`:

```csharp
builder.Services.AddSingleton<VlanService>();      // Shared instance
builder.Services.AddTransient<MainPage>();          // New instance per request
builder.Services.AddTransient<SettingsPage>();
```

Access services in pages:
```csharp
var service = Handler?.MauiContext?.Services.GetService<VlanService>();
```

### Navigation

Using `NavigationPage`:
```csharp
await Navigation.PushAsync(new SettingsPage());  // Navigate forward
await Navigation.PopAsync();                      // Navigate back
```

## Code Organization

### Adding a New Model

1. Create class in `Models/` folder
2. Define properties with getters/setters
3. Consider implementing `INotifyPropertyChanged` for data binding

### Adding a New Service

1. Create class in `Services/` folder
2. Register in `MauiProgram.cs`
3. Choose appropriate lifetime:
   - `Singleton`: One instance for app lifetime
   - `Transient`: New instance each time
   - `Scoped`: One instance per scope (rarely used in MAUI)

### Adding a New Page

1. Create XAML and code-behind files in `Pages/` folder
2. Register in `MauiProgram.cs` as transient
3. Add navigation from existing pages

Example XAML structure:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="NbgDev.SwitchMan.Pages.MyPage"
             Title="My Page">
    <!-- Content here -->
</ContentPage>
```

## Styling

### Global Styles

- **Colors**: `Resources/Styles/Colors.xaml`
- **Control Styles**: `Resources/Styles/Styles.xaml`

### Page-Specific Styles

Add to page's XAML:
```xml
<ContentPage.Resources>
    <ResourceDictionary>
        <Style x:Key="MyStyle" TargetType="Label">
            <Setter Property="TextColor" Value="Blue" />
        </Style>
    </ResourceDictionary>
</ContentPage.Resources>
```

## Platform-Specific Code

### Conditional Compilation

```csharp
#if ANDROID
    // Android-specific code
#elif WINDOWS
    // Windows-specific code
#endif
```

### Platform Files

- **Android**: `Platforms/Android/`
  - `MainActivity.cs`: Main activity
  - `AndroidManifest.xml`: App permissions and settings
  
- **Windows**: `Platforms/Windows/`
  - `App.xaml`: WinUI app definition
  - `Package.appxmanifest`: Windows app manifest

## Testing

### Manual Testing

1. **Windows**: Run from Visual Studio or:
   ```bash
   dotnet run -f net10.0-windows10.0.19041.0
   ```

2. **Android**: Ensure emulator/device is running, then:
   ```bash
   dotnet build -f net10.0-android -t:Run
   ```

### Testing Checklist

- [ ] Main page loads correctly
- [ ] Settings button navigates to settings page
- [ ] Can add VLAN with valid name and ID
- [ ] Validation prevents invalid VLAN IDs
- [ ] VLANs appear in the list
- [ ] Swipe-to-delete works correctly
- [ ] Navigation back button works
- [ ] App survives rotation (Android)
- [ ] App works in light/dark mode

## Debugging

### Visual Studio

- Set breakpoints in code
- Use Debug β†' Start Debugging (F5)
- View output in Output window

### Common Issues

**MAUI workload not found**
```bash
dotnet workload install maui
```

**Build errors after pulling changes**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Android deployment fails**
- Ensure Android emulator is running
- Check Android SDK is installed
- Verify `adb devices` shows connected device

## Best Practices

1. **XAML Naming**:
   - Use `x:Name` for controls accessed in code-behind
   - Use PascalCase for names

2. **Async Operations**:
   - Use `async`/`await` for UI operations
   - Don't block UI thread

3. **Resource Management**:
   - Dispose of resources when done
   - Unsubscribe from events

4. **Accessibility**:
   - Use `SemanticProperties` for screen readers
   - Set minimum touch targets (44x44)

5. **Performance**:
   - Use `CollectionView` instead of `ListView`
   - Implement virtualization for large lists
   - Avoid complex layouts in list items

## Build Configuration

### Debug vs Release

- **Debug**: Includes debug symbols, no optimization
- **Release**: Optimized, smaller size, ready for distribution

```bash
# Debug build (default)
dotnet build

# Release build
dotnet build -c Release
```

### Publishing

**Windows**:
```bash
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

**Android**:
```bash
dotnet publish -f net10.0-android -c Release
```

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
fix: resolve navigation crash on Android
docs: update development guide
```

### Branch Strategy

- `main`: Stable releases
- `develop`: Integration branch
- `feature/*`: New features
- `fix/*`: Bug fixes

## Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [XAML Documentation](https://docs.microsoft.com/dotnet/desktop/wpf/xaml/)
- [C# Guide](https://docs.microsoft.com/dotnet/csharp/)
- [GitHub Copilot Instructions](.github/copilot-instructions.md)

## Getting Help

- Check existing issues on GitHub
- Consult the .NET MAUI documentation
- Ask in team chat or discussions
- Create a new issue for bugs or feature requests
