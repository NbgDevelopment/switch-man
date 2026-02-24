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
    /// Port name
    /// </summary>
    public string PortName { get; set; } = string.Empty;
    
    /// <summary>
    /// VLAN ID assigned to this port
    /// </summary>
    public int VlanId { get; set; }

    /// <summary>
    /// VLAN name assigned to this port
    /// </summary>
    public string VlanName { get; set; } = string.Empty;

    public PortInfo()
    {
    }

    public PortInfo(int portNumber, string portName, int vlanId, string vlanName)
    {
        PortNumber = portNumber;
        PortName = portName;
        VlanId = vlanId;
        VlanName = vlanName;
    }
}
