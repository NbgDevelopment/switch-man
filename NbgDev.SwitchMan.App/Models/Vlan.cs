namespace NbgDev.SwitchMan.App.Models;

public class Vlan
{
    public string Name { get; set; } = string.Empty;

    public Vlan()
    {
    }

    public Vlan(string name)
    {
        Name = name;
    }
}
