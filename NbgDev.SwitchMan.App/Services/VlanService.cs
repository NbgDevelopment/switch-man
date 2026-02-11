using System.Collections.ObjectModel;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class VlanService
{
    private readonly ObservableCollection<Vlan> _vlans = new();
    private readonly IConfigurationService _configurationService;

    public VlanService(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        
        // Load existing configuration at startup
        var loadedVlans = _configurationService.LoadConfiguration();
        foreach (var vlan in loadedVlans)
        {
            _vlans.Add(vlan);
        }
    }

    public ObservableCollection<Vlan> GetVlans()
    {
        return _vlans;
    }

    public void AddVlan(Vlan vlan)
    {
        if (_vlans.Any(v => v.VlanId == vlan.VlanId))
        {
            throw new InvalidOperationException($"A VLAN with ID {vlan.VlanId} already exists.");
        }
        _vlans.Add(vlan);
        _configurationService.SaveConfiguration(_vlans);
    }

    public void RemoveVlan(Vlan vlan)
    {
        _vlans.Remove(vlan);
        _configurationService.SaveConfiguration(_vlans);
    }

    public void UpdateVlan(Vlan oldVlan, Vlan newVlan)
    {
        var index = _vlans.IndexOf(oldVlan);
        if (index >= 0)
        {
            // Check if the new VLAN ID already exists (excluding the current VLAN being updated)
            if (newVlan.VlanId != oldVlan.VlanId && _vlans.Any(v => v.VlanId == newVlan.VlanId))
            {
                throw new InvalidOperationException($"A VLAN with ID {newVlan.VlanId} already exists.");
            }
            _vlans[index] = newVlan;
            _configurationService.SaveConfiguration(_vlans);
        }
    }
}
