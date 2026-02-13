# Switch Man

A .NET Blazor Server application for managing VLAN configurations on network switches.

## Overview

Switch Man is a web-based application built with .NET 10 Blazor Server that provides a simple interface for managing VLAN configurations. The app allows users to define VLANs with names and IDs, which can later be used to configure individual ports on managed network switches.

## Features

- **Mobile-First Design**: Optimized for smartphone and tablet use with touch-friendly controls
- **Web-Based Interface**: Access from any browser on desktop or mobile devices
- **Simple Home Page**: Landing page with clear app title and access to settings
- **VLAN Management**: Add, view, and delete VLANs with ease
- **Switch Management**: Add network switches and retrieve port information via SNMP
- **Input Validation**: Ensures VLAN IDs are within the valid range (1-4094)
- **Real-time Updates**: Blazor Server provides real-time UI updates
- **Delete Functionality**: Simple button-based deletion of VLANs and switches
- **Persistent Storage**: VLANs and switches are stored in JSON format and persist across restarts
- **Configurable Storage Path**: Storage location can be customized via configuration
- **SNMP Integration**: Retrieve port count and VLAN assignments from network switches

## Platform Support

- **Cross-Platform**: Runs in Docker on any platform
- **Web-Based**: Access from any modern web browser on desktop, tablet, or smartphone
- **Mobile-Optimized**: Touch-friendly interface designed for use on mobile devices

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
β"œβ"€β"€ Models/              # Data models (Vlan, Switch)
β"œβ"€β"€ Services/            # Business logic (VlanService, SwitchService, ConfigurationService)
β"œβ"€β"€ config/              # Configuration storage (JSON files)
└── Program.cs           # Application entry point

NbgDev.SwitchMan.Switches.Contract/
└── ISwitchAccessService # Interface for switch access
    └── Models/          # Port information models

NbgDev.SwitchMan.Switches.TLSG2008/
└── Implementation       # SNMP-based implementation for TL-SG2008 switches
```

## Usage

1. **Access the app** - Navigate to http://localhost:8080 in your browser
2. **Home page** - You'll see the landing page with the app title
3. **Open Settings** - Click the "Open Settings" button or use the navigation menu
4. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
5. **View VLANs** - All configured VLANs appear in the list on the right
6. **Delete VLANs** - Click the "Delete" button next to any VLAN
7. **Add Switches** - Enter switch name and IP address, then click "Add Switch"
8. **View Switch Information** - Port count and VLAN assignments are retrieved and logged automatically

### Switch Configuration Requirements

To enable SNMP access on your TL-SG2008 switch:

1. **Enable SNMP on the switch**:
   - Log into the switch's web interface (default: http://192.168.0.1)
   - Navigate to **System Tools** > **SNMP Config**
   - Set **SNMP** to **Enable**
   - Configure the **SNMP Community String** (default is "public")
   - Click **Apply** to save settings

2. **Network Requirements**:
   - Ensure the switch is accessible from the host running Switch Man
   - The switch must respond to SNMP requests on UDP port 161
   - Firewall rules should allow SNMP traffic between Switch Man and the switch

3. **Supported Switches**:
   - Currently supports TP-Link TL-SG2008 switches
   - Uses SNMP v1 protocol with community string authentication
   - Default community string: "public" (configurable in future versions)

4. **Troubleshooting**:
   - If you receive timeout errors when adding a switch:
     - Verify SNMP is enabled on the switch
     - Check the community string matches (default: "public")
     - Ensure network connectivity between Switch Man and the switch
     - Verify no firewall is blocking UDP port 161
     - Test SNMP access using tools like `snmpwalk` (Linux/Mac) or SNMP Tester (Windows)

**Example SNMP test command** (Linux/Mac):
```bash
snmpwalk -v1 -c public <switch-ip-address> system
```

### Configuration

The application stores VLAN and switch configurations in JSON format. By default, configurations are stored in the `config` directory within the application folder.

**Environment Variables:**

- `SwitchMan__ConfigPath`: Override the default configuration directory path
  - Default: `config` (relative to application directory)
  - Example: `/data/config` for a custom absolute path

**Configuration File:**

The application creates `vlans.json` and `switches.json` files in the configured directory:

**vlans.json:**
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

**switches.json:**
```json
[
  {
    "Name": "Main Switch",
    "IpAddress": "192.168.1.10"
  },
  {
    "Name": "Access Switch",
    "IpAddress": "192.168.1.20"
  }
]
```

## Current Limitations

- Only supports TL-SG2008 switches via SNMP v1
- SNMP community string is hardcoded to "public"
- No authentication or multi-user support
- Switch port configuration (changing VLANs) not yet implemented

## Future Enhancements

- Configurable SNMP community strings
- Support for additional switch models
- Switch port configuration (assign VLANs to ports)
- SNMP v2c and v3 support
- SSH-based switch management
- Port-to-VLAN mapping interface
- Switch discovery and selection
- Import/export configurations
- Multi-switch management
- User authentication and authorization
- Database backend (SQLite, SQL Server, or MongoDB)

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.