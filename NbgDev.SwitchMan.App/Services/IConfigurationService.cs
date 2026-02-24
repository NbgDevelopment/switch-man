using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public interface IConfigurationService
{
    List<Switch> LoadSwitches();
    void SaveSwitches(IEnumerable<Switch> switches);
    OmadaSettings? LoadOmadaSettings();
    void SaveOmadaSettings(OmadaSettings settings);
}
