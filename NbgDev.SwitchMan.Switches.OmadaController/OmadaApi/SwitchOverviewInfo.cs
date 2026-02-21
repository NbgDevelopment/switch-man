namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="Mac">Device MAC</param>
/// <param name="Ip">Device IP</param>
internal record SwitchOverviewInfo(string Mac, string Ip, SwitchPortInfo[] PortList);
