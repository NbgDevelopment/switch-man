# Switch Man

A .NET Blazor Server application for managing VLAN configurations on network switches.

## Overview

Switch Man is a web-based application built with .NET 10 Blazor Server that provides a simple interface for managing VLAN configurations. The app allows users to define VLANs with names and IDs, which can later be used to configure individual ports on managed network switches.

## Features

- **Mobile-First Design**: Optimized for smartphone and tablet use with touch-friendly controls
- **Web-Based Interface**: Access from any browser on desktop or mobile devices
- **Simple Home Page**: Landing page with clear app title and access to settings
- **VLAN Management**: Add, view, and delete VLANs with ease
- **Switch Management**: Add network switches and retrieve port information via Omada Controller API
- **Input Validation**: Ensures VLAN IDs are within the valid range (1-4094)
- **Real-time Updates**: Blazor Server provides real-time UI updates
- **Delete Functionality**: Simple button-based deletion of VLANs and switches
- **Persistent Storage**: VLANs and switches are stored in JSON format and persist across restarts
- **Configurable Storage Path**: Storage location can be customized via configuration
- **Omada Controller Integration**: Retrieve port count and VLAN assignments from switches managed by TP-Link Omada Controller

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

NbgDev.SwitchMan.Switches.OmadaController/
└── Implementation       # OAuth 2.0 API-based implementation for TP-Link Omada Controller
```

## Usage

1. **Access the app** - Navigate to http://localhost:8080 in your browser
2. **Home page** - You'll see the landing page with the app title
3. **Open Settings** - Click the "Open Settings" button or use the navigation menu
4. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
5. **View VLANs** - All configured VLANs appear in the list on the right
6. **Delete VLANs** - Click the "Delete" button next to any VLAN
7. **Add Switches** - Enter switch name and IP address, then click "Add Switch"
8. **View Switch Information** - Port count and VLAN assignments are retrieved from Omada Controller and logged automatically

### Omada Controller Configuration

To use switch access via Omada Controller:

1. **Prerequisites**:
   - TP-Link Omada Controller must be running and accessible via HTTPS
   - Switches must be adopted and managed by the Omada Controller
   - Create an OpenAPI application in the controller to get OAuth 2.0 credentials

2. **Create OpenAPI Application**:
   - Log into your Omada Controller
   - Navigate to **Settings** > **Platform Integration** > **Open API**
   - Click **Add Application** to create a new OpenAPI app
   - Give it a descriptive name (e.g., "Switch Man")
   - Copy the **Client ID** and **Client Secret** (shown once only - save them securely!)
   - **Important**: The Client Secret is only displayed once during creation

3. **Find Your Omada ID**:
   - Log in to your Omada Controller web interface
   - Look at the browser URL: `https://<controller-ip>:8043/<omadaId>/...`
   - The Omada ID is the long hexadecimal string in the URL path (e.g., `1234567890abcdef1234567890abcdef`)
   - Alternatively, you can find it in the controller's site settings

4. **Configuration via Environment Variables**:
   - `OmadaController__ControllerUrl` - Controller URL (e.g., `https://192.168.1.100:8043`)
   - `OmadaController__OmadaId` - Omada Controller ID (32-character hexadecimal - required)
   - `OmadaController__ClientId` - OAuth 2.0 Client ID from OpenAPI app (required)
   - `OmadaController__ClientSecret` - OAuth 2.0 Client Secret from OpenAPI app (required)
   - `OmadaController__AllowInvalidCertificate` - Set to `true` for self-signed certificates (default: `false`)

5. **Example Docker Configuration**:
   ```bash
   # With valid SSL certificate (production - default)
   docker run -d -p 8080:8080 \
     -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
     -e OmadaController__OmadaId=1234567890abcdef1234567890abcdef \
     -e OmadaController__ClientId=your-client-id \
     -e OmadaController__ClientSecret=your-client-secret \
     --name switchman switchman:latest
   
   # With self-signed certificate (development/testing only)
   docker run -d -p 8080:8080 \
     -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
     -e OmadaController__OmadaId=1234567890abcdef1234567890abcdef \
     -e OmadaController__ClientId=your-client-id \
     -e OmadaController__ClientSecret=your-client-secret \
     -e OmadaController__AllowInvalidCertificate=true \
     --name switchman switchman:latest
   ```

6. **Network Requirements**:
   - Controller must be accessible via HTTPS from the host running Switch Man
   - Firewall rules should allow HTTPS traffic to the controller (default port 8043)
   - Controller OpenAPI must be enabled

7. **Security Best Practices**:
   - Create a dedicated OpenAPI application for Switch Man (don't reuse credentials)
   - Use strong, unique Client Secrets
   - Store credentials securely using environment variables or secrets management
   - In production, always use valid SSL certificates (`AllowInvalidCertificate=false`)
   - Regularly rotate OpenAPI credentials
   - Monitor OpenAPI application access logs in the controller

8. **Advantages of Omada Controller Access**:
   - Centralized management of multiple switches
   - No need to configure SNMP on each switch individually
   - Works with any switch model supported by Omada Controller
   - Access to additional management features through the controller
   - OAuth 2.0 authentication provides better security than SNMP community strings
   - Automatic token refresh for long-running operations

**For detailed OpenAPI setup and permissions**: See [Omada User Setup Guide](./OMADA_USER_SETUP.md)

### Configuration

The application stores VLAN and switch configurations in JSON format. By default, configurations are stored in the `config` directory within the application folder.

**Environment Variables:**

Application Configuration:
- `SwitchMan__ConfigPath`: Override the default configuration directory path
  - Default: `config` (relative to application directory)
  - Example: `/data/config` for a custom absolute path

Omada Controller Configuration (when using Omada Controller access):
- `OmadaController__ControllerUrl`: Controller URL
  - Default: `https://localhost:8043`
  - Example: `https://192.168.1.100:8043`
- `OmadaController__OmadaId`: Omada Controller ID (32-character hexadecimal identifier)
  - Required: Must be obtained from controller URL or settings
  - Example: `1234567890abcdef1234567890abcdef`
- `OmadaController__ClientId`: OAuth 2.0 Client ID from OpenAPI app
  - Required: Must be obtained from Omada Controller (Settings > Platform Integration > Open API)
  - Example: `your-client-id-here`
- `OmadaController__ClientSecret`: OAuth 2.0 Client Secret from OpenAPI app
  - Required: Must be obtained from Omada Controller (Settings > Platform Integration > Open API)
  - **Important**: Shown only once during OpenAPI app creation - save securely!
  - Example: `your-client-secret-here`
- `OmadaController__AllowInvalidCertificate`: Allow self-signed/invalid SSL certificates
  - Default: `false` (secure by default)
  - Set to `true` to bypass SSL certificate validation for self-signed certificates
  - **WARNING**: Only use this in development/testing environments. Setting this to `true` in production is a security risk.

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

- Switch access via TP-Link Omada Controller only (OAuth 2.0 OpenAPI)
- Omada Controller credentials must be configured via environment variables or appsettings.json
- No authentication or multi-user support in the Switch Man application
- Switch port configuration (changing VLANs) not yet implemented
- Read-only access to switch port information

## Future Enhancements

- Switch port configuration (assign VLANs to ports)
- Port-to-VLAN mapping interface
- Support for multiple Omada Controller instances
- Switch discovery and auto-detection
- Import/export configurations
- Multi-switch management dashboard
- User authentication and authorization
- Database backend (SQLite, SQL Server, or MongoDB)
- Support for additional switch access methods (SNMP, SSH)

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.