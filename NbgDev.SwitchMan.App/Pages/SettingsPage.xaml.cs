using System.Collections.ObjectModel;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly VlanService _vlanService;
    public ObservableCollection<Vlan> Vlans { get; set; }

    public SettingsPage()
    {
        InitializeComponent();
        
        // Get the service from the service provider
        _vlanService = Handler?.MauiContext?.Services.GetService<VlanService>() 
                      ?? throw new InvalidOperationException("VlanService not found in dependency injection container.");
        
        Vlans = _vlanService.GetVlans();
        BindingContext = this;
    }

    private async void OnAddVlanClicked(object sender, EventArgs e)
    {
        var name = VlanNameEntry.Text?.Trim();
        var vlanIdText = VlanIdEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Error", "Please enter a VLAN name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(vlanIdText) || !int.TryParse(vlanIdText, out int vlanId))
        {
            await DisplayAlert("Error", "Please enter a valid VLAN ID.", "OK");
            return;
        }

        if (vlanId < 1 || vlanId > 4094)
        {
            await DisplayAlert("Error", "VLAN ID must be between 1 and 4094.", "OK");
            return;
        }

        try
        {
            var vlan = new Vlan(name, vlanId);
            _vlanService.AddVlan(vlan);

            VlanNameEntry.Text = string.Empty;
            VlanIdEntry.Text = string.Empty;

            await DisplayAlert("Success", $"VLAN '{name}' (ID: {vlanId}) added successfully.", "OK");
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnDeleteVlan(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is Vlan vlan)
        {
            bool confirm = await DisplayAlert("Confirm Delete", 
                $"Are you sure you want to delete VLAN '{vlan.Name}' (ID: {vlan.VlanId})?", 
                "Yes", "No");
            
            if (confirm)
            {
                _vlanService.RemoveVlan(vlan);
            }
        }
    }
}
