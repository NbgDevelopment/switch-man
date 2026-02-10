# Switch Man - GitHub Copilot Instructions

## Project Overview
Switch Man is a .NET MAUI application designed to manage VLAN configurations for network switches. The app allows users to configure individual ports of a managed network switch to specific VLAN IDs.

## Technology Stack
- **.NET 10**: Latest .NET framework
- **.NET MAUI**: Cross-platform UI framework
- **Target Platforms**: Windows and Android only
- **Language**: C# with implicit usings and nullable reference types enabled

## Project Structure
```
NbgDev.SwitchMan/
β"œβ"€β"€ Models/
β"‚   └── Vlan.cs                  # VLAN data model
β"œβ"€β"€ Services/
β"‚   └── VlanService.cs           # In-memory VLAN management service
β"œβ"€β"€ Pages/
β"‚   β"œβ"€β"€ MainPage.xaml/cs         # Main application page
β"‚   └── SettingsPage.xaml/cs    # VLAN management settings page
β"œβ"€β"€ Resources/
β"‚   β"œβ"€β"€ Styles/
β"‚   β"‚   β"œβ"€β"€ Colors.xaml          # Application color definitions
β"‚   β"‚   └── Styles.xaml          # Application-wide styles
β"‚   β"œβ"€β"€ Fonts/                   # Application fonts
β"‚   β"œβ"€β"€ AppIcon/                 # App icon assets
β"‚   └── Splash/                  # Splash screen assets
β"œβ"€β"€ Platforms/
β"‚   β"œβ"€β"€ Android/                 # Android-specific code
β"‚   └── Windows/                 # Windows-specific code
β"œβ"€β"€ App.xaml/cs                   # Application entry point
β"œβ"€β"€ MauiProgram.cs               # MAUI configuration and DI setup
└── NbgDev.SwitchMan.csproj      # Project file
```

## Architecture

### Data Model
- **Vlan**: Simple POCO class with `Name` (string) and `VlanId` (int) properties

### Services
- **VlanService**: Singleton service managing an in-memory ObservableCollection of VLANs
  - Methods: `GetVlans()`, `AddVlan()`, `RemoveVlan()`, `UpdateVlan()`
  - No persistence - data is lost when app closes

### Pages
- **MainPage**: Landing page with app title and "Open Settings" button
- **SettingsPage**: VLAN management interface with:
  - Form to add new VLANs (name + VLAN ID)
  - List display of configured VLANs using CollectionView
  - Swipe-to-delete functionality for removing VLANs
  - Input validation (VLAN ID must be 1-4094)

## Dependency Injection
Services and pages are registered in `MauiProgram.cs`:
- VlanService: Registered as singleton
- MainPage and SettingsPage: Registered as transient

## Coding Standards
1. **Use implicit usings** - enabled in project file
2. **Nullable reference types** - enabled in project file
3. **XAML Conventions**:
   - Use `x:Name` for controls that need code-behind access
   - Leverage data binding where appropriate
   - Use semantic properties for accessibility
4. **C# Conventions**:
   - Follow standard .NET naming conventions
   - Use async/await for UI operations
   - Validate user input before processing

## VLAN ID Validation Rules
- VLAN ID must be numeric
- Valid range: 1-4094 (standard IEEE 802.1Q range)
- Display appropriate error messages for invalid input

## Navigation
- Uses NavigationPage for page navigation
- MainPage opens SettingsPage via `Navigation.PushAsync()`

## Common Development Tasks

### Adding a New Page
1. Create XAML and code-behind files in `/Pages` directory
2. Register page in `MauiProgram.cs` as transient service
3. Add navigation logic from existing pages

### Adding a New Service
1. Create service class in `/Services` directory
2. Register in `MauiProgram.cs` with appropriate lifetime (singleton/transient/scoped)
3. Inject via constructor or access via `Handler?.MauiContext?.Services`

### Modifying UI Styles
- Global colors: Edit `Resources/Styles/Colors.xaml`
- Control styles: Edit `Resources/Styles/Styles.xaml`
- Page-specific styles: Add to page's XAML ResourceDictionary

### Platform-Specific Code
- Android: Modify files in `Platforms/Android/`
- Windows: Modify files in `Platforms/Windows/`
- Use conditional compilation: `#if ANDROID`, `#if WINDOWS`

## Building and Running
```bash
# Restore dependencies
dotnet restore

# Build for Android
dotnet build -f net10.0-android

# Build for Windows
dotnet build -f net10.0-windows10.0.19041.0

# Run on Android emulator
dotnet build -f net10.0-android -t:Run

# Run on Windows
dotnet build -f net10.0-windows10.0.19041.0 -t:Run
```

## Known Limitations
- VLAN data is stored in memory only (no persistence)
- Only supports Windows and Android platforms
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
- **Build errors**: Ensure .NET 10 SDK is installed and MAUI workload is available
- **Service not found**: Check service registration in `MauiProgram.cs`
- **Navigation issues**: Verify page is wrapped in NavigationPage in `App.xaml.cs`
