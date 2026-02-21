using System.Net;

namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="Name">Device name</param>
/// <param name="Type">Device type</param>
/// <param name="Mac">Device MAC</param>
/// <param name="Ip">Device IP</param>
internal record DeviceInfo(string Name, string Type, string Mac, string Ip)
{
    internal IPAddress IpAddress { get; } = IPAddress.Parse(Ip);
}
