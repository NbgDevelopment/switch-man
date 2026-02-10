# Switch Man - GitHub Copilot Instructions

## Project Overview
Switch Man is a .NET Blazor Server application designed to manage VLAN configurations for network switches. The app allows users to configure individual ports of a managed network switch to specific VLAN IDs.

## Technology Stack
- **.NET 10**: Latest .NET framework
- **Blazor Server**: Server-side Blazor for interactive web UI
- **Target Platform**: Cross-platform (runs in Docker)
- **Language**: C# with implicit usings and nullable reference types enabled

## Project Structure
```
NbgDev.SwitchMan.App/
β"œβ"€β"€ Components/
β"‚   β"œβ"€β"€ Layout/
β"‚   β"‚   β"œβ"€β"€ MainLayout.razor     # Main layout
β"‚   β"‚   └── NavMenu.razor        # Navigation menu
β"‚   └── Pages/
β"‚       β"œβ"€β"€ Home.razor            # Landing page
β"‚       └── Settings.razor        # VLAN management page
β"œβ"€β"€ Models/
β"‚   └── Vlan.cs                   # VLAN data model
β"œβ"€β"€ Services/
β"‚   └── VlanService.cs            # In-memory VLAN management service
└── Program.cs                     # Application entry point
```

## Architecture

### Data Model
- **Vlan**: Simple POCO class with `Name` (string) and `VlanId` (int) properties

### Services
- **VlanService**: Singleton service managing an in-memory ObservableCollection of VLANs
  - Methods: `GetVlans()`, `AddVlan()`, `RemoveVlan()`, `UpdateVlan()`
  - Registered as singleton in dependency injection
  - No persistence - data is lost when app restarts

### Pages
- **Home**: Landing page with app title and link to settings
- **Settings**: VLAN management interface with:
  - Form to add new VLANs (name + VLAN ID)
  - List display of configured VLANs
  - Delete button for removing VLANs
  - Input validation (VLAN ID must be 1-4094)
  - Real-time updates using Blazor Server

## Coding Standards
1. **Use implicit usings** - enabled in project file
2. **Nullable reference types** - enabled in project file
3. **Razor Conventions**:
   - Use `@page` directive for routable components
   - Use `@inject` for dependency injection
   - Use `@rendermode InteractiveServer` for interactive components
4. **C# Conventions**:
   - Follow standard .NET naming conventions
   - Use async/await for async operations
   - Validate user input before processing

## VLAN ID Validation Rules
- VLAN ID must be numeric
- Valid range: 1-4094 (standard IEEE 802.1Q range)
- Display appropriate error messages for invalid input

## Navigation
- Uses Blazor routing with `NavLink` components
- Home page accessible at `/`
- Settings page accessible at `/settings`

## Common Development Tasks

### Adding a New Page
1. Create `.razor` file in `Components/Pages/` directory
2. Add `@page "/route"` directive
3. Add navigation link in `NavMenu.razor`

### Adding a New Service
1. Create service class in `Services/` directory
2. Register in `Program.cs` with appropriate lifetime
3. Inject into components with `@inject`

### Modifying UI
- Bootstrap 5 is available for styling
- Update components in `Components/` directory
- Use Blazor data binding with `@bind`

## Building and Running

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run locally
dotnet run --project NbgDev.SwitchMan.App

# Run with Docker
docker build -t switchman:latest .
docker run -d -p 8080:8080 switchman:latest
```

## Docker Deployment

The application is designed to run in Docker:
- Dockerfile is at repository root
- Exposes port 8080
- Uses multi-stage build for smaller images
- Based on official Microsoft .NET images

## Known Limitations
- VLAN data is stored in memory only (no persistence)
- Runs on any platform via Docker
- No network switch integration (UI only)
- No authentication or user management

## Future Enhancement Ideas
- Add persistent storage (SQLite, SQL Server, or MongoDB)
- Implement actual network switch communication (SNMP, SSH)
- Add port-to-VLAN mapping functionality
- Support for switch discovery and selection
- Import/export VLAN configurations
- Multi-switch management
- User authentication and authorization

## Troubleshooting
- **Build errors**: Ensure .NET 10 SDK is installed
- **Service not found**: Check service registration in `Program.cs`
- **Page not routing**: Verify `@page` directive is present
- **Docker issues**: Ensure Docker is running and port 8080 is available
