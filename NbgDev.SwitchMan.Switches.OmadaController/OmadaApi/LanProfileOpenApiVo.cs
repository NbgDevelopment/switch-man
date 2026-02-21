namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="Id">LAN profile ID</param>
/// <param name="Name">Name should contain 1 to 128 characters.</param>
/// <param name="NativeNetworkId">Native network ID, Native Network cannot be selected from Tagged Networks or Untagged Networks.</param>
internal record LanProfileOpenApiVo(string Id, string Name, string NativeNetworkId);
