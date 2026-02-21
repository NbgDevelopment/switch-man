namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="Id">LAN network ID</param>
/// <param name="Name">LAN network name should contain 1 to 128 characters.</param>
/// <param name="VlanType">When purpose is interface, VLANType is valid. 0: Single; 1: Multiple</param>
/// <param name="Vlan">When purpose is "VLAN" or purpose is "interface" and VLANType is 0, create VLAN. VLAN range 1 to 4090.</param>
internal record LanNetworkQueryOpenApiVo(string Id, string Name, int VlanType, int Vlan);
