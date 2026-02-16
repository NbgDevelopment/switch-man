## Omada Controller Configuration Example

To use the Omada Controller implementation instead of direct SNMP access, update `Program.cs`:

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

When using Docker:
```bash
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__Username=myadmin \
  -e OmadaController__Password=mypassword \
  --name switchman switchman:latest
```

When using appsettings.json:
```json
{
  "OmadaController": {
    "ControllerUrl": "https://192.168.1.100:8043",
    "Username": "myadmin",
    "Password": "mypassword"
  }
}
```

### Default Values

If no configuration is provided, the following defaults are used:
- ControllerUrl: `https://localhost:8043`
- Username: `admin`
- Password: `admin`
