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
- **Persistent Storage**: VLANs are stored in JSON format and persist across restarts
- **Configurable Storage Path**: Storage location can be customized via configuration

## Platform Support

- **Cross-Platform**: Runs in Docker on any platform
- **Web-Based**: Access from any modern web browser

## Requirements

- .NET 10 SDK (for development)
- Docker (for deployment)

## CI/CD

This project uses GitHub Actions for automated building, testing, and Docker image publishing. See [WORKFLOWS.md](WORKFLOWS.md) for details on:
- Running workflows manually from any branch
- GitVersioning configuration
- Published Docker images

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

# Run container with default configuration
docker run -d -p 8080:8080 --name switchman switchman:latest

# Run container with custom configuration path
docker run -d -p 8080:8080 \
  -e SwitchMan__ConfigPath=/data/config \
  -v /path/on/host:/data/config \
  --name switchman switchman:latest

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
β"œβ"€β"€ Services/            # Business logic (VlanService, ConfigurationService)
β"œβ"€β"€ config/              # VLAN configuration storage (JSON files)
└── Program.cs           # Application entry point
```

## Usage

1. **Access the app** - Navigate to http://localhost:8080 in your browser
2. **Home page** - You'll see the landing page with the app title
3. **Open Settings** - Click the "Open Settings" button or use the navigation menu
4. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
5. **View VLANs** - All configured VLANs appear in the list on the right
6. **Delete VLANs** - Click the "Delete" button next to any VLAN

### Configuration

The application stores VLAN configurations in JSON format. By default, configurations are stored in the `config` directory within the application folder.

**Environment Variables:**

- `SwitchMan__ConfigPath`: Override the default configuration directory path
  - Default: `config` (relative to application directory)
  - Example: `/data/config` for a custom absolute path

**Configuration File:**

The application creates a `vlans.json` file in the configured directory with the following structure:

```json
[
  {
    "Name": "Management",
    "VlanId": 10
  },
  {
    "Name": "Guest",
    "VlanId": 20
  }
]
```

## Current Limitations

- No network switch integration yet
- No authentication or multi-user support

## Future Enhancements

- Network switch communication (SNMP, SSH)
- Port-to-VLAN mapping
- Switch discovery and selection
- Import/export configurations
- Multi-switch management
- User authentication and authorization
- Database backend (SQLite, SQL Server, or MongoDB)

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.