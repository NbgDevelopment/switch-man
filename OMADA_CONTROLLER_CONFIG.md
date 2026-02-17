## Omada Controller Configuration Example

To use the Omada Controller implementation instead of direct SNMP access, you need to configure OAuth 2.0 authentication with the Omada Controller's OpenAPI.

### Prerequisites

**Important**: The Omada Controller uses OAuth 2.0 for OpenAPI access. You need to create an OpenAPI application in your Omada Controller to obtain the Client ID and Client Secret.

#### Creating an OpenAPI Application

1. Log in to your Omada Controller web interface
2. Navigate to **Settings** β†' **Platform Integration** β†' **Open API**
3. Click **Add New App** or **Create Application**
4. Configure the application:
   - **App Name**: `SwitchMan` (or your preferred name)
   - **Access Mode**: Select **Client Credentials** (for server-to-server access)
   - **Privileges**: Grant necessary permissions (see [OMADA_USER_SETUP.md](./OMADA_USER_SETUP.md) for details)
5. Click **Save** or **Create**
6. **Important**: Copy the **Client ID** and **Client Secret** - you'll need these for configuration
7. The Client Secret is only shown once - store it securely!

### Option 1: Using TL-SG2008 (Direct SNMP)
```csharp
// Register switch access service (current implementation)
builder.Services.AddTlSg2008SwitchAccess();
```

### Option 2: Using Omada Controller OpenAPI
```csharp
using NbgDev.SwitchMan.Switches.OmadaController;

// Register switch access service via Omada Controller
builder.Services.AddOmadaControllerSwitchAccess(builder.Configuration);
```

### Environment Variables for Omada Controller

**Authentication**: Uses OAuth 2.0 Client Credentials flow with OpenAPI. Create an OpenAPI app in your controller (Settings > Platform Integration > Open API) to obtain credentials.

**SSL Certificate Validation**: By default, Switch Man validates SSL certificates. If your Omada Controller uses a self-signed certificate, you can disable validation by setting `AllowInvalidCertificate` to `true`. **WARNING**: Only use this in development/testing environments.

When using Docker:
```bash
# With valid SSL certificate (recommended for production)
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__ClientId=your-client-id \
  -e OmadaController__ClientSecret=your-client-secret \
  --name switchman switchman:latest

# With self-signed certificate (development/testing only)
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__ClientId=your-client-id \
  -e OmadaController__ClientSecret=your-client-secret \
  -e OmadaController__AllowInvalidCertificate=true \
  --name switchman switchman:latest
```

When using appsettings.json:
```json
{
  "OmadaController": {
    "ControllerUrl": "https://192.168.1.100:8043",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "AllowInvalidCertificate": false
  }
}
```

**For self-signed certificates in development/testing**:
```json
{
  "OmadaController": {
    "ControllerUrl": "https://192.168.1.100:8043",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "AllowInvalidCertificate": true
  }
}
```

### Default Values

If no configuration is provided, the following defaults are used:
- ControllerUrl: `https://localhost:8043`
- ClientId: `""` (empty - must be configured)
- ClientSecret: `""` (empty - must be configured)
- AllowInvalidCertificate: `false` (SSL certificate validation enabled)

### Security Notes

- **Never commit credentials**: Use environment variables or user secrets for ClientId and ClientSecret
- **Use strong secrets**: The Client Secret is a sensitive credential - treat it like a password
- **Rotate credentials**: Regularly rotate your OpenAPI application credentials
- **Limit privileges**: Grant only the minimum required permissions to the OpenAPI app (see [OMADA_USER_SETUP.md](./OMADA_USER_SETUP.md))
- **Enable SSL validation in production**: Only disable certificate validation for development/testing

### Troubleshooting

**Authentication Errors**:
- Verify the Client ID and Client Secret are correct
- Ensure the OpenAPI application is enabled in the Omada Controller
- Check that the application has sufficient privileges
- Review the Omada Controller logs for authentication failures

**SSL Certificate Errors**:
- If using self-signed certificates, set `AllowInvalidCertificate=true` (development only)
- For production, install a valid SSL certificate or add the CA certificate to your system's trust store

**API Access Errors**:
- Verify network connectivity to the controller
- Check firewall rules allow HTTPS traffic on port 8043
- Ensure the Omada Controller OpenAPI is enabled
