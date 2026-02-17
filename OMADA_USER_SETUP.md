# Omada Controller OpenAPI Setup Guide

This guide explains how to configure the TP-Link Omada Controller's OpenAPI to work with Switch Man using OAuth 2.0 authentication.

## Overview

Switch Man uses the Omada Controller's **OpenAPI** with **OAuth 2.0 Client Credentials** authentication to:
- **Current Operations**: Read switch information, retrieve port configurations and VLAN assignments
- **Future Operations**: Configure port VLAN assignments and manage switch ports

The OpenAPI uses a Client ID and Client Secret for authentication, which is more secure than username/password authentication.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Creating an OpenAPI Application](#creating-an-openapi-application)
3. [Required Permissions - Current Features](#required-permissions---current-features)
4. [Required Permissions - Future Features](#required-permissions---future-features)
5. [Security Best Practices](#security-best-practices)
6. [Troubleshooting](#troubleshooting)

## Prerequisites

- Access to Omada Controller as an administrator
- Omada Controller version 5.0 or higher (with OpenAPI support)
- Network connectivity to the controller
- HTTPS access to the controller (typically port 8043)

## Creating an OpenAPI Application

The Omada Controller uses OAuth 2.0 for API authentication. Follow these steps to create an OpenAPI application:

### Step 1: Access OpenAPI Settings

1. Log in to your Omada Controller web interface as an administrator
2. Navigate to **Settings** β†' **Platform Integration** β†' **Open API**
3. You should see the OpenAPI management page

### Step 2: Create New Application

1. Click **Add New App** or **Create Application** button
2. Fill in the application details:

**Basic Information:**
- **App Name**: `SwitchMan` (or your preferred descriptive name)
- **Description**: `Network switch management via Switch Man` (optional)

**Access Mode:**
- Select **Client Credentials** (This is for server-to-server authentication without user interaction)
- Do NOT use Authorization Code mode - that's for user-interactive applications

### Step 3: Configure Privileges

Grant the following permissions based on your needs:

#### Option A: Minimal Permissions (Recommended for Production)

Select only the permissions needed for Switch Man's current operations:

**For Current Read-Only Operations:**
- βœ… **Switches**: View/Read access
- βœ… **Switch Ports**: View/Read access  
- βœ… **VLANs**: View/Read access

**For Future Write Operations (Port Configuration):**
- βœ… **Switch Ports**: Modify/Write access
- βœ… **VLANs**: Modify/Write access (if needed)

#### Option B: Full Access (For Testing/Development)

For testing or development environments, you can grant full API access, but this is NOT recommended for production.

### Step 4: Save and Obtain Credentials

1. Click **Save** or **Create** to create the application
2. **IMPORTANT**: The system will display the **Client ID** and **Client Secret**
3. **Copy both values immediately** - the Client Secret is only shown once!
4. Store these credentials securely (use a password manager or secure vault)

Example credentials (yours will be different):
```
Client ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890
Client Secret: 1234567890abcdef1234567890abcdef12345678
```

### Step 5: Configure Switch Man

Use the credentials in your Switch Man configuration:

**Via Environment Variables:**
```bash
export OmadaController__ClientId="your-client-id"
export OmadaController__ClientSecret="your-client-secret"
export OmadaController__ControllerUrl="https://192.168.1.100:8043"
```

**Via appsettings.json:**
```json
{
  "OmadaController": {
    "ControllerUrl": "https://192.168.1.100:8043",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

**Via Docker:**
```bash
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__ClientId=your-client-id \
  -e OmadaController__ClientSecret=your-client-secret \
  --name switchman switchman:latest
```

## Required Permissions - Current Features

Switch Man currently performs **read-only** operations on the Omada Controller. The OpenAPI application needs the following access:

### Minimum Permissions Table

| Resource Category | Required Permission | Purpose |
|-------------------|-------------------|---------|
| **Switches** | Read/View | List and view switch information |
| **Switch Ports** | Read/View | Read port settings and VLAN assignments |
| **VLANs** | Read/View | Read VLAN configuration |

### API Operations Used

Current implementation uses these Omada Controller OpenAPI endpoints:

- `POST /openapi/authorize/token` - OAuth 2.0 authentication
- `GET /openapi/v1/sites/{site}/switches` - List switches and basic information
- `GET /openapi/v1/sites/{site}/switches/{switchId}/ports` - Retrieve port configurations

## Required Permissions - Future Features

When Switch Man adds the ability to configure port VLANs, additional permissions will be required:

### Extended Permissions Table

| Resource Category | Required Permission | Purpose |
|-------------------|-------------------|---------|
| **Switch Ports** | Write/Modify | Change port settings |
| **Switch Ports** | Configure | Apply VLAN assignments to ports |
| **VLANs** | Write/Modify | Create/modify VLANs if needed |

### Future API Operations

Planned features will use these additional OpenAPI endpoints:

- `POST /openapi/v1/sites/{site}/switches/{switchId}/ports/{portId}` - Configure individual port
- `PATCH /openapi/v1/sites/{site}/switches/{switchId}/ports/{portId}/vlan` - Update port VLAN assignment
- `PUT /openapi/v1/sites/{site}/switches/{switchId}/ports/bulk` - Bulk port configuration

## Security Best Practices

### 1. Use Dedicated OpenAPI Applications

- βœ… **DO**: Create a dedicated OpenAPI application named `SwitchMan` with minimal permissions
- ❌ **DON'T**: Reuse OpenAPI credentials across multiple applications
- βœ… **DO**: Use descriptive names for your applications to track usage

### 2. Secure Credential Management

- **Client Secret is sensitive**: Treat it like a password - it's only shown once during creation
- **Use strong secrets**: The Omada Controller generates secure Client Secrets automatically
- **Store securely**: Use environment variables, secrets management systems, or secure vaults
- **Never commit to source control**: Add `appsettings.json` with secrets to `.gitignore`
- **Rotate credentials**: Create new OpenAPI applications periodically and delete old ones

### 3. Environment Variable Configuration

Configure credentials using environment variables (recommended):

```bash
# Docker
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__ClientId=your-client-id \
  -e OmadaController__ClientSecret=your-client-secret \
  --name switchman switchman:latest

# Kubernetes Secret (recommended for production)
kubectl create secret generic switchman-omada-creds \
  --from-literal=controller-url=https://192.168.1.100:8043 \
  --from-literal=client-id=your-client-id \
  --from-literal=client-secret=your-client-secret

# User Secrets (for local development)
dotnet user-secrets set "OmadaController:ClientId" "your-client-id"
dotnet user-secrets set "OmadaController:ClientSecret" "your-client-secret"
```

### 4. Network Security

- Use HTTPS for all controller communications (enforced by Omada Controller)
- Consider network segmentation to isolate management traffic
- Use firewall rules to restrict API access to authorized hosts
- Monitor OpenAPI access logs in the Omada Controller

### 5. Regular Security Audits

- Review OpenAPI applications quarterly
- Remove unused or old applications
- Rotate credentials periodically (create new app, update config, delete old app)
- Monitor API access logs for unusual activity
- Disable unused applications promptly

### 6. Least Privilege Principle

- Start with read-only permissions for initial deployment
- Only add write permissions when port configuration features are needed
- Regularly review and minimize permissions based on actual usage

## Troubleshooting

### Authentication Failures

**Problem**: `401 Unauthorized` or "Failed to obtain access token" errors

**Solutions**:
1. Verify Client ID and Client Secret are correct (check for copy/paste errors)
2. Ensure the OpenAPI application is enabled in the Omada Controller
3. Check that the application hasn't been deleted or disabled
4. Verify the controller URL is correct and accessible
5. Check Omada Controller logs: **System** β†' **Logs** β†' **Controller Logs**
6. Ensure the OpenAPI feature is enabled in your Omada Controller version

**Common Causes**:
- Incorrect Client ID or Client Secret
- Application deleted or disabled
- OpenAPI not enabled on the controller
- Network connectivity issues

### Permission Denied Errors

**Problem**: `403 Forbidden` when accessing switch information

**Solutions**:
1. Verify the OpenAPI application has required permissions (see Required Permissions section)
2. Check that the application has access to the correct site(s)
3. Review the privilege settings for the application
4. Ensure the switches are properly adopted in the controller

### Connection Issues

**Problem**: Unable to connect to Omada Controller

**Solutions**:
1. Verify the controller URL format: `https://host:port` (typically port 8043)
2. Check firewall rules allow HTTPS traffic on port 8043
3. Confirm the Omada Controller service is running
4. Test connectivity: `curl -k https://192.168.1.100:8043`
5. **SSL Certificate Issues**: If using self-signed certificates, see below

### SSL Certificate Issues

**Problem**: SSL certificate validation errors or untrusted certificate warnings

**Solutions**:

For **production environments** (recommended):
1. Install a valid SSL certificate on the Omada Controller
2. Use a certificate from a trusted Certificate Authority (CA)
3. Configure proper DNS and certificate subject names

For **development/testing environments** (use with caution):
1. Set `OmadaController__AllowInvalidCertificate=true` to bypass certificate validation
2. Example Docker command:
   ```bash
   docker run -d -p 8080:8080 \
     -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
     -e OmadaController__ClientId=your-client-id \
     -e OmadaController__ClientSecret=your-client-secret \
     -e OmadaController__AllowInvalidCertificate=true \
     --name switchman switchman:latest
   ```
3. Example appsettings.json:
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

**WARNING**: Disabling certificate validation (`AllowInvalidCertificate=true`) should only be used in:
- Development environments
- Testing environments
- Private networks where security is managed at the network level

Never use this setting in production without understanding the security implications. This makes your connection vulnerable to man-in-the-middle attacks.

### Token Expiration

**Problem**: API calls fail after some time

**Solutions**:
1. Switch Man automatically handles token refresh
2. If issues persist, check the Omada Controller session timeout settings
3. Verify system clocks are synchronized (important for token validation)

## Additional Resources

- [TP-Link Omada Controller User Guide](https://www.tp-link.com/support/download/)
- [Omada Controller API Documentation](https://www.tp-link.com/omada-sdn/)
- [Switch Man Configuration Guide](./OMADA_CONTROLLER_CONFIG.md)
- [Switch Man README](./README.md)

## Version Compatibility

This guide applies to:
- **Omada Controller Software**: Version 5.0 and higher
- **Omada Hardware Controller**: OC200, OC300 (firmware 1.x and higher)
- **Switch Man**: Current version and future releases

**Note**: Permission names and locations may vary slightly between Omada Controller versions. Consult your specific version's documentation if the menu structure differs from this guide.

## Support

If you encounter issues not covered in this guide:

1. Check the [Switch Man Issues](https://github.com/NbgDevelopment/switch-man/issues) on GitHub
2. Review Omada Controller logs in **System** β†' **Logs** β†' **Controller Logs**
3. Enable debug logging in Switch Man for detailed error messages
4. Open a new issue with detailed error messages and configuration details (redact sensitive information)
