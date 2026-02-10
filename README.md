# Switch Man

A .NET Blazor Server application for managing VLAN configurations on network switches.

## Overview

Switch Man is a web-based application built with .NET 10 Blazor Server that provides a simple interface for managing VLAN configurations. The app allows users to define VLANs with names and IDs, which can later be used to configure individual ports on managed network switches.

## Features

- **Web-Based Interface**: Access from any browser
- **Simple Home Page**: Landing page with clear app title and access to settings
- **VLAN Management**: Add, view, and delete VLANs with ease
- **Input Validation**: Ensures VLAN IDs are within the valid range (1-4094)
- **Real-time Updates**: Blazor Server provides real-time UI updates
- **Delete Functionality**: Simple button-based deletion of VLANs
- **In-Memory Storage**: VLANs are stored in memory during the app session (no persistence yet)

## Platform Support

- **Cross-Platform**: Runs in Docker on any platform
- **Web-Based**: Access from any modern web browser

## Requirements

- .NET 10 SDK (for development)
- Docker (for deployment)

## Building

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run locally
dotnet run --project NbgDev.SwitchMan.App
```

## Running with Docker

```bash
# Build Docker image
docker build -t switchman:latest .

# Run container
docker run -d -p 8080:8080 --name switchman switchman:latest

# Access the application
# Open your browser to http://localhost:8080
```

## Project Structure

```
NbgDev.SwitchMan.App/
β"œβ"€β"€ Components/
β"‚   β"œβ"€β"€ Layout/          # Layout components (NavMenu, MainLayout)
β"‚   └── Pages/           # Blazor pages (Home, Settings)
β"œβ"€β"€ Models/              # Data models (Vlan)
β"œβ"€β"€ Services/            # Business logic (VlanService)
└── Program.cs           # Application entry point
```

## Usage

1. **Access the app** - Navigate to http://localhost:8080 in your browser
2. **Home page** - You'll see the landing page with the app title
3. **Open Settings** - Click the "Open Settings" button or use the navigation menu
4. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
5. **View VLANs** - All configured VLANs appear in the list on the right
6. **Delete VLANs** - Click the "Delete" button next to any VLAN

## Current Limitations

- VLANs are stored in memory only (cleared when app restarts)
- No network switch integration yet
- No authentication or multi-user support
- No persistent storage

## Future Enhancements

- Persistent storage (SQLite or database)
- Network switch communication (SNMP, SSH)
- Port-to-VLAN mapping
- Switch discovery and selection
- Import/export configurations
- Multi-switch management
- User authentication and authorization

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.