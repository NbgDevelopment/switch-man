# Switch Man

A .NET WPF application for managing VLAN configurations on network switches.

## Overview

Switch Man is a Windows desktop application built with .NET 10 WPF that provides a simple interface for managing VLAN configurations. The app allows users to define VLANs with names and IDs, which can later be used to configure individual ports on managed network switches.

## Features

- **Simple Main Window**: Landing page with clear app title and access to settings
- **VLAN Management**: Add, view, and delete VLANs with ease
- **Input Validation**: Ensures VLAN IDs are within the valid range (1-4094)
- **Delete Functionality**: Simple button-based deletion of VLANs
- **In-Memory Storage**: VLANs are stored in memory during the app session (no persistence yet)

## Platform Support

- **Windows** (.NET 10 WPF application)

## Requirements

- .NET 10 SDK
- Windows operating system

## Installation

1. Install .NET 10 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/)

## Building

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project NbgDev.SwitchMan.App
```

## Project Structure

```
NbgDev.SwitchMan.App/
β"œβ"€β"€ Models/           # Data models (Vlan)
β"œβ"€β"€ Services/         # Business logic (VlanService)
β"œβ"€β"€ Windows/          # WPF windows (MainWindow, SettingsWindow)
└── App.xaml          # Application entry point
```

## Usage

1. **Launch the app** - You'll see the main window with the app title
2. **Open Settings** - Click the "Open Settings" button
3. **Add VLANs** - Enter a VLAN name and ID (1-4094), then click "Add VLAN"
4. **View VLANs** - All configured VLANs appear in the list
5. **Delete VLANs** - Select a VLAN and click "Delete Selected"

## Current Limitations

- VLANs are stored in memory only (cleared when app closes)
- No network switch integration yet
- No authentication or multi-user support

## Future Enhancements

- Persistent storage (SQLite or local file)
- Network switch communication (SNMP, SSH)
- Port-to-VLAN mapping
- Switch discovery and selection
- Import/export configurations
- Multi-switch management

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

See [LICENSE](LICENSE) file for details.