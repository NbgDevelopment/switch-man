namespace NbgDev.SwitchMan.Switches.Contract.Models;

/// <summary>
/// Represents a VLAN that can be assigned to a switch port
/// </summary>
public class VlanInfo
{
    /// <summary>
    /// VLAN profile ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// VLAN name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public VlanInfo()
    {
    }

    public VlanInfo(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
