## Omada Controller Configuration Example

To use the Omada Controller implementation instead of direct SNMP access, update `Program.cs`:

### Prerequisites

**Important**: For security best practices, create a dedicated user account with minimal permissions instead of using the default admin account. See the [Omada User Setup Guide](./OMADA_USER_SETUP.md) for detailed instructions.

### Option 1: Using TL-SG2008 (Direct SNMP)
```csharp
// Register switch access service (current implementation)
builder.Services.AddTlSg2008SwitchAccess();
```

### Option 2: Using Omada Controller
```csharp
using NbgDev.SwitchMan.Switches.OmadaController;

// Register switch access service via Omada Controller
builder.Services.AddOmadaControllerSwitchAccess(builder.Configuration);
```

### Environment Variables for Omada Controller

**Security Note**: Use a dedicated service account instead of the admin account. See [OMADA_USER_SETUP.md](./OMADA_USER_SETUP.md) for creating a user with minimal required permissions.

When using Docker:
```bash
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__Username=switchman-api \
  -e OmadaController__Password=your-secure-password \
  --name switchman switchman:latest
```

When using appsettings.json:
```json
{
  "OmadaController": {
    "ControllerUrl": "https://192.168.1.100:8043",
    "Username": "switchman-api",
    "Password": "your-secure-password"
  }
}
```

### Default Values

If no configuration is provided, the following defaults are used:
- ControllerUrl: `https://localhost:8043`
- Username: `admin`
- Password: `admin`
