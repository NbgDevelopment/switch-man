# Switch Man

A .NET MAUI application for managing VLAN configurations on network switches.

## Overview

Switch Man is a cross-platform application built with .NET 10 MAUI that provides a simple interface for managing VLAN configurations. The app allows users to define VLANs with names and IDs, which can later be used to configure individual ports on managed network switches.

## Features

- **Simple Main Interface**: Landing page with clear app title and access to settings
- **VLAN Management**: Add, view, and delete VLANs with ease
- **Input Validation**: Ensures VLAN IDs are within the valid range (1-4094)
- **Swipe to Delete**: Intuitive gesture-based deletion of VLANs
- **In-Memory Storage**: VLANs are stored in memory during the app session (no persistence yet)

## Platform Support

- **Windows** (10.0.17763.0 or higher)
- **Android** (API 21 or higher)

## Requirements

- .NET 10 SDK
- .NET MAUI workload installed
- For Windows: Windows 10/11 with WinUI 3 support
- For Android: Android SDK and emulator/device

## Installation

1. Install .NET 10 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
2. Install the MAUI workload:
   ```bash
   dotnet workload install maui
   ```

## Building

```bash
# Restore dependencies
dotnet restore

# Build for Windows
dotnet build -f net10.0-windows10.0.19041.0

# Build for Android  
dotnet build -f net10.0-android
```

## Running

### Windows
```bash
dotnet run -f net10.0-windows10.0.19041.0
```

### Android
```bash
# With an emulator or device connected
dotnet build -f net10.0-android -t:Run
```

## Project Structure

```
NbgDev.SwitchMan.App/
β"œβ"€β"€ Models/           # Data models (Vlan)
β"œβ"€β"€ Services/         # Business logic (VlanService)
β"œβ"€β"€ Pages/            # UI pages (MainPage, SettingsPage)
β"œβ"€β"€ Resources/        # App resources (fonts, images, styles)
└── Platforms/        # Platform-specific code
```

## Usage

1. **Launch the app** - You'll see the main screen with the app title
2. **Open Settings** - Click the "Open Settings" button
3. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
4. **View VLANs** - All configured VLANs appear in the list below
5. **Delete VLANs** - Swipe left on any VLAN and tap "Delete"

## Current Limitations

- VLANs are stored in memory only (cleared when app closes)
- No network switch integration yet
- No authentication or multi-user support

## Future Enhancements

- Persistent storage (SQLite or local preferences)
- Network switch communication (SNMP, SSH)
- Port-to-VLAN mapping
- Switch discovery and selection
- Import/export configurations
- Multi-switch management

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.