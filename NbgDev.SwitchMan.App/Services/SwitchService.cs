using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.Switches.Contract;

namespace NbgDev.SwitchMan.App.Services;

public class SwitchService
{
    private readonly ObservableCollection<Switch> _switches = new();
    private readonly IConfigurationService _configurationService;
    private readonly ISwitchAccessService _switchAccessService;
    private readonly ILogger<SwitchService> _logger;

    public SwitchService(
        IConfigurationService configurationService,
        ISwitchAccessService switchAccessService,
        ILogger<SwitchService> logger)
    {
        _configurationService = configurationService;
        _switchAccessService = switchAccessService;
        _logger = logger;
        
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

    public async Task AddSwitchAsync(Switch sw)
    {
        try
        {
            _logger.LogInformation("Adding switch {Name} at {IpAddress}", sw.Name, sw.IpAddress);
            
            // Retrieve port information from the switch
            var portCount = await _switchAccessService.GetPortCountAsync(sw.IpAddress);
            _logger.LogInformation("Switch {Name} has {PortCount} ports", sw.Name, portCount);
            
            var portVlans = await _switchAccessService.GetPortVlansAsync(sw.IpAddress);
            foreach (var portInfo in portVlans)
            {
                _logger.LogInformation("Switch {Name} - Port {Port}: VLAN {VlanId}", 
                    sw.Name, portInfo.PortNumber, portInfo.VlanId);
            }
            
            _switches.Add(sw);
            _configurationService.SaveSwitches(_switches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding switch {Name} at {IpAddress}", sw.Name, sw.IpAddress);
            throw;
        }
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
