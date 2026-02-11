using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public interface IConfigurationService
{
    List<Vlan> LoadConfiguration();
    void SaveConfiguration(IEnumerable<Vlan> vlans);
}
