using System.Collections.ObjectModel;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class VlanService
{
    private readonly ObservableCollection<Vlan> _vlans = new();

    public ObservableCollection<Vlan> GetVlans()
    {
        return _vlans;
    }

    public void AddVlan(Vlan vlan)
    {
        _vlans.Add(vlan);
    }

    public void RemoveVlan(Vlan vlan)
    {
        _vlans.Remove(vlan);
    }

    public void UpdateVlan(Vlan oldVlan, Vlan newVlan)
    {
        var index = _vlans.IndexOf(oldVlan);
        if (index >= 0)
        {
            _vlans[index] = newVlan;
        }
    }
}
