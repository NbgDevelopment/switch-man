namespace NbgDev.SwitchMan.App.Models;

public class Switch
{
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;

    public Switch()
    {
    }

    public Switch(string name, string ipAddress)
    {
        Name = name;
        IpAddress = ipAddress;
    }
}
