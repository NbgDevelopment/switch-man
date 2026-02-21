using NbgDev.SwitchMan.Switches.Contract.Models;

namespace NbgDev.SwitchMan.Switches.Contract;

/// <summary>
/// Service interface for accessing network switches
/// </summary>
public interface ISwitchAccessService
{
    /// <summary>
    /// Gets the number of ports on the switch
    /// </summary>
    /// <param name="ipAddress">IP address of the switch</param>
    /// <returns>Number of ports</returns>
    Task<int> GetPortCountAsync(string ipAddress);

    /// <summary>
    /// Gets VLAN information for all ports on the switch
    /// </summary>
    /// <param name="ipAddress">IP address of the switch</param>
    /// <returns>Collection of port information including VLAN assignments</returns>
    Task<IEnumerable<PortInfo>> GetPortVlansAsync(string ipAddress);
}
