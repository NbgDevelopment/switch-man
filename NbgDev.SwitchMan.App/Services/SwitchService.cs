using System.Collections.ObjectModel;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class SwitchService
{
    private readonly ObservableCollection<Switch> _switches = new();
    private readonly IConfigurationService _configurationService;

    public SwitchService(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        
        // Load existing switches at startup
        var loadedSwitches = _configurationService.LoadSwitches();
        foreach (var sw in loadedSwitches)
        {
            _switches.Add(sw);
        }
    }

    public ObservableCollection<Switch> GetSwitches()
    {
        return _switches;
    }

    public void AddSwitch(Switch sw)
    {
        _switches.Add(sw);
        _configurationService.SaveSwitches(_switches);
    }

    public void RemoveSwitch(Switch sw)
    {
        _switches.Remove(sw);
        _configurationService.SaveSwitches(_switches);
    }

    public void MoveUp(Switch sw)
    {
        var index = _switches.IndexOf(sw);
        if (index > 0)
        {
            _switches.Move(index, index - 1);
            _configurationService.SaveSwitches(_switches);
        }
    }

    public void MoveDown(Switch sw)
    {
        var index = _switches.IndexOf(sw);
        if (index >= 0 && index < _switches.Count - 1)
        {
            _switches.Move(index, index + 1);
            _configurationService.SaveSwitches(_switches);
        }
    }
}
