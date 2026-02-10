namespace NbgDev.SwitchMan.Models;

public class Vlan
{
    public string Name { get; set; } = string.Empty;
    public int VlanId { get; set; }

    public Vlan()
    {
    }

    public Vlan(string name, int vlanId)
    {
        Name = name;
        VlanId = vlanId;
    }
}
