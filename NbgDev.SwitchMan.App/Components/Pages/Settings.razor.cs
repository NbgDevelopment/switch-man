using Microsoft.AspNetCore.Components;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Components.Pages;

public partial class Settings
{
    [Inject]
    private VlanService VlanService { get; set; } = null!;

    private string vlanName = string.Empty;
    private int vlanId;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;

    private void AddVlan()
    {
        errorMessage = string.Empty;
        successMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(vlanName))
        {
            errorMessage = "Please enter a VLAN name.";
            return;
        }

        if (vlanId < 1 || vlanId > 4094)
        {
            errorMessage = "VLAN ID must be between 1 and 4094.";
            return;
        }

        try
        {
            var vlan = new Vlan(vlanName, vlanId);
            VlanService.AddVlan(vlan);

            successMessage = $"VLAN '{vlanName}' (ID: {vlanId}) added successfully.";
            vlanName = string.Empty;
            vlanId = 0;
        }
        catch (InvalidOperationException ex)
        {
            errorMessage = ex.Message;
        }
    }

    private void DeleteVlan(Vlan vlan)
    {
        VlanService.RemoveVlan(vlan);
        successMessage = $"VLAN '{vlan.Name}' deleted successfully.";
        errorMessage = string.Empty;
    }
}
