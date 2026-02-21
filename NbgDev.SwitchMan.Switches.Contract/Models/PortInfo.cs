namespace NbgDev.SwitchMan.Switches.Contract.Models;

/// <summary>
/// Represents information about a switch port
/// </summary>
public class PortInfo
{
    /// <summary>
    /// Port number (1-based)
    /// </summary>
    public int PortNumber { get; set; }
    
    /// <summary>
    /// VLAN ID assigned to this port
    /// </summary>
    public int VlanId { get; set; }

    public PortInfo()
    {
    }

    public PortInfo(int portNumber, int vlanId)
    {
        PortNumber = portNumber;
        VlanId = vlanId;
    }
}
