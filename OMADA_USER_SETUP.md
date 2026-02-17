# Omada Controller User Setup Guide

This guide explains how to create a dedicated user account in TP-Link Omada Controller with the minimal permissions required for Switch Man to function properly.

## Overview

Switch Man requires API access to the Omada Controller to:
- **Current Operations**: Read switch information, retrieve port configurations and VLAN assignments
- **Future Operations**: Configure port VLAN assignments and manage switch ports

For security best practices, it's recommended to create a dedicated service account rather than using the default admin account.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Creating a Dedicated User Account](#creating-a-dedicated-user-account)
3. [Required Permissions - Current Features](#required-permissions---current-features)
4. [Required Permissions - Future Features](#required-permissions---future-features)
5. [Security Best Practices](#security-best-practices)
6. [Troubleshooting](#troubleshooting)

## Prerequisites

- Access to Omada Controller as an administrator
- Omada Controller version 5.x or higher (recommended)
- Network connectivity to the controller

## Creating a Dedicated User Account

### Step 1: Access User Management

1. Log in to your Omada Controller web interface
2. Navigate to **Settings** β†' **Administrators**
3. Click **Create New Administrator**

### Step 2: Configure Basic Account Details

Fill in the following information:

- **Username**: `switchman-api` (or your preferred name)
- **Email**: Provide a valid email address for notifications
- **Password**: Use a strong password (minimum 8 characters, mix of uppercase, lowercase, numbers, special characters)
- **Confirm Password**: Re-enter the password
- **Account Status**: Enabled

### Step 3: Assign Permissions

The Omada Controller uses role-based access control. You have two options:

#### Option A: Create Custom Role (Recommended for Production)

Create a custom role with minimal required permissions:

1. Go to **Settings** β†' **Roles**
2. Click **Create New Role**
3. Name: `Switch Man API Access`
4. Configure the following permissions:

**Current Required Permissions:**
- βœ… **Switch**: View Devices
- βœ… **Switch**: View Port Configuration
- βœ… **Network**: View VLANs
- βœ… **System**: API Access

**Additional Permissions for Future Features:**
- βœ… **Switch**: Modify Port Configuration
- βœ… **Switch**: Configure Ports
- βœ… **Network**: Manage VLANs

5. Save the custom role
6. Assign this role to the `switchman-api` user

#### Option B: Use Built-in Role (For Testing/Development)

For testing or development environments, you can use a built-in role:

- **Viewer Role**: Provides read-only access (sufficient for current features)
- **Operator Role**: Provides read and limited write access (needed for future port configuration features)

**Note**: The Operator role may grant more permissions than necessary, so the custom role approach is recommended for production.

### Step 4: Configure Access Scope (Site Selection)

1. In the user configuration, select which sites this user can access
2. For most deployments, select **All Sites** or the specific site where your switches are managed
3. This ensures the API user can access all switches you want to manage with Switch Man

### Step 5: Save and Verify

1. Click **Save** to create the user
2. Note down the username and password for configuration in Switch Man
3. Test the login by signing out and logging back in with the new credentials

## Required Permissions - Current Features

Switch Man currently performs **read-only** operations on the Omada Controller. The following permissions are required:

### Minimum Permissions Table

| Permission Category | Required Permission | Purpose |
|-------------------|-------------------|---------|
| **Switches** | View Devices | List and view switch information |
| **Switches** | View Port Configuration | Read port settings and VLAN assignments |
| **Network** | View VLANs | Read VLAN configuration |
| **System** | API Access | Enable API authentication and requests |

### API Operations Used

Current implementation uses these Omada Controller API endpoints:

- `GET /api/v2/sites/{site}/switches` - List switches and basic information
- `GET /api/v2/sites/{site}/switches/{switchId}/ports` - Retrieve port configurations
- `GET /api/v2/login` - Authenticate and obtain access token

## Required Permissions - Future Features

When Switch Man adds the ability to configure port VLANs, additional permissions will be required:

### Extended Permissions Table

| Permission Category | Required Permission | Purpose |
|-------------------|-------------------|---------|
| **Switches** | Modify Port Configuration | Change port settings |
| **Switches** | Configure Ports | Apply VLAN assignments to ports |
| **Network** | Manage VLANs | Create/modify VLANs if needed |

### Future API Operations

Planned features will use these additional endpoints:

- `POST /api/v2/sites/{site}/switches/{switchId}/ports/{portId}` - Configure individual port
- `PATCH /api/v2/sites/{site}/switches/{switchId}/ports/{portId}/vlan` - Update port VLAN assignment
- `PUT /api/v2/sites/{site}/switches/{switchId}/ports/bulk` - Bulk port configuration

## Security Best Practices

### 1. Use Dedicated Service Accounts

- βœ… **DO**: Create a dedicated `switchman-api` user with minimal permissions
- ❌ **DON'T**: Use the default admin account for API access

### 2. Strong Password Requirements

- Use passwords with at least 16 characters
- Include uppercase, lowercase, numbers, and special characters
- Store passwords securely using environment variables or secrets management
- Never commit passwords to source control

### 3. Environment Variable Configuration

Configure credentials using environment variables (recommended):

```bash
# Docker
docker run -d -p 8080:8080 \
  -e OmadaController__ControllerUrl=https://192.168.1.100:8043 \
  -e OmadaController__Username=switchman-api \
  -e OmadaController__Password=your-secure-password \
  --name switchman switchman:latest

# Kubernetes Secret (recommended for production)
kubectl create secret generic switchman-omada-creds \
  --from-literal=controller-url=https://192.168.1.100:8043 \
  --from-literal=username=switchman-api \
  --from-literal=password=your-secure-password
```

### 4. Network Security

- Use HTTPS for all controller communications
- Consider network segmentation to isolate management traffic
- Use firewall rules to restrict API access to authorized hosts
- Enable two-factor authentication on the Omada Controller when possible

### 5. Regular Security Audits

- Review user permissions quarterly
- Rotate API credentials periodically (e.g., every 90 days)
- Monitor API access logs for unusual activity
- Disable unused accounts promptly

### 6. Least Privilege Principle

- Start with read-only permissions for initial deployment
- Only add write permissions when port configuration features are needed
- Regularly review and minimize permissions based on actual usage

## Troubleshooting

### Authentication Failures

**Problem**: `401 Unauthorized` or authentication errors

**Solutions**:
1. Verify username and password are correct
2. Check that the user account is enabled
3. Ensure API access is enabled for the user role
4. Verify the controller URL is accessible
5. Check that SSL certificate validation issues aren't blocking the connection

### Permission Denied Errors

**Problem**: `403 Forbidden` when accessing switch information

**Solutions**:
1. Verify the user has "View Devices" permission
2. Check that the user has access to the correct site(s)
3. Ensure "API Access" permission is granted
4. Review the role assignment for the user

### Connection Issues

**Problem**: Unable to connect to Omada Controller

**Solutions**:
1. Verify the controller URL format: `https://host:port` (typically port 8043)
2. Check firewall rules allow HTTPS traffic on port 8043
3. Confirm the Omada Controller service is running
4. Test connectivity: `curl -k https://192.168.1.100:8043/api/info`
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
     -e OmadaController__Username=switchman-api \
     -e OmadaController__Password=your-password \
     -e OmadaController__AllowInvalidCertificate=true \
     --name switchman switchman:latest
   ```
3. Example appsettings.json:
   ```json
   {
     "OmadaController": {
       "ControllerUrl": "https://192.168.1.100:8043",
       "Username": "switchman-api",
       "Password": "your-password",
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
