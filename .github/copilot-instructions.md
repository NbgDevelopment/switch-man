# Switch Man - GitHub Copilot Instructions

## Project Overview
Switch Man is a .NET WPF application designed to manage VLAN configurations for network switches. The app allows users to configure individual ports of a managed network switch to specific VLAN IDs.

## Technology Stack
- **.NET 10**: Latest .NET framework
- **WPF**: Windows Presentation Foundation
- **Target Platform**: Windows only
- **Language**: C# with implicit usings and nullable reference types enabled

## Project Structure
```
NbgDev.SwitchMan.App/
β"œβ"€β"€ Models/
β"‚   └── Vlan.cs                  # VLAN data model
β"œβ"€β"€ Services/
β"‚   └── VlanService.cs           # In-memory VLAN management service
β"œβ"€β"€ Windows/
β"‚   β"œβ"€β"€ MainWindow.xaml/cs       # Main application window
β"‚   └── SettingsWindow.xaml/cs  # VLAN management settings window
β"œβ"€β"€ App.xaml/cs                   # Application entry point
└── NbgDev.SwitchMan.App.csproj  # Project file
```

## Architecture

### Data Model
- **Vlan**: Simple POCO class with `Name` (string) and `VlanId` (int) properties

### Services
- **VlanService**: Service managing an in-memory ObservableCollection of VLANs
  - Methods: `GetVlans()`, `AddVlan()`, `RemoveVlan()`, `UpdateVlan()`
  - No persistence - data is lost when app closes

### Windows
- **MainWindow**: Landing window with app title and "Open Settings" button
- **SettingsWindow**: VLAN management interface with:
  - Form to add new VLANs (name + VLAN ID)
  - ListBox display of configured VLANs
  - Delete selected functionality for removing VLANs
  - Input validation (VLAN ID must be 1-4094)

## Coding Standards
1. **Use implicit usings** - enabled in project file
2. **Nullable reference types** - enabled in project file
3. **XAML Conventions**:
   - Use `x:Name` for controls that need code-behind access
   - Leverage data binding where appropriate
   - Use semantic element names
4. **C# Conventions**:
   - Follow standard .NET naming conventions
   - Use MessageBox for user notifications
   - Validate user input before processing

## VLAN ID Validation Rules
- VLAN ID must be numeric
- Valid range: 1-4094 (standard IEEE 802.1Q range)
- Display appropriate error messages for invalid input

## Navigation
- Uses modal dialogs for windows
- MainWindow opens SettingsWindow via `ShowDialog()`

## Common Development Tasks

### Adding a New Window

1. Create XAML and code-behind files in `/Windows` directory
2. Add window opening logic from existing windows
3. Use `ShowDialog()` for modal, `Show()` for modeless

### Adding a New Service

1. Create service class in `/Services` directory
2. Instantiate in windows as needed
3. Use `ObservableCollection<T>` for bindable data

### Modifying UI Styles

- Define styles in Window.Resources or App.xaml
- Use standard WPF styling approaches

## Building and Running

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project NbgDev.SwitchMan.App
```

## Known Limitations
- VLAN data is stored in memory only (no persistence)
- Only supports Windows platform
- No network switch integration (UI only)
- No authentication or user management

## Future Enhancement Ideas
- Add persistence (SQLite, preferences, or file storage)
- Implement actual network switch communication (SNMP, SSH)
- Add port-to-VLAN mapping functionality
- Support for switch discovery and selection
- Import/export VLAN configurations
- Multi-switch management

## Troubleshooting
- **Build errors**: Ensure .NET 10 SDK is installed
- **Service not found**: Check service instantiation in window constructors
- **Window issues**: Verify XAML markup is valid
