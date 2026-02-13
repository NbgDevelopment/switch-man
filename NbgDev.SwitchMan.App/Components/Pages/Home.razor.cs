using Microsoft.AspNetCore.Components;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Components.Pages;

public partial class Home
{
    [Inject]
    private SwitchService SwitchService { get; set; } = null!;

    private bool showAddDialog = false;
    private string switchName = string.Empty;
    private string ipAddress = string.Empty;
    private string errorMessage = string.Empty;

    private void OpenAddDialog()
    {
        showAddDialog = true;
        switchName = string.Empty;
        ipAddress = string.Empty;
        errorMessage = string.Empty;
    }

    private void CloseAddDialog()
    {
        showAddDialog = false;
        switchName = string.Empty;
        ipAddress = string.Empty;
        errorMessage = string.Empty;
    }

    private async Task AddSwitch()
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(switchName))
        {
            errorMessage = "Please enter a switch name.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            errorMessage = "Please enter an IP address.";
            return;
        }

        if (!System.Net.IPAddress.TryParse(ipAddress, out _))
        {
            errorMessage = "Please enter a valid IP address.";
            return;
        }

        try
        {
            var newSwitch = new Switch(switchName, ipAddress);
            await SwitchService.AddSwitchAsync(newSwitch);
            CloseAddDialog();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error adding switch: {ex.Message}";
        }
    }

    private void MoveUp(Switch sw)
    {
        SwitchService.MoveUp(sw);
    }

    private void MoveDown(Switch sw)
    {
        SwitchService.MoveDown(sw);
    }

    private void DeleteSwitch(Switch sw)
    {
        SwitchService.RemoveSwitch(sw);
    }
}
