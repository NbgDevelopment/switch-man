using System.Windows;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Windows;

public partial class SettingsWindow : Window
{
    private readonly VlanService _vlanService;

    public SettingsWindow()
    {
        InitializeComponent();
        
        _vlanService = new VlanService();
        VlanListBox.ItemsSource = _vlanService.GetVlans();
    }

    private void OnAddVlanClicked(object sender, RoutedEventArgs e)
    {
        var name = VlanNameTextBox.Text?.Trim();
        var vlanIdText = VlanIdTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Please enter a VLAN name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(vlanIdText) || !int.TryParse(vlanIdText, out int vlanId))
        {
            MessageBox.Show("Please enter a valid VLAN ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (vlanId < 1 || vlanId > 4094)
        {
            MessageBox.Show("VLAN ID must be between 1 and 4094.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            var vlan = new Vlan(name, vlanId);
            _vlanService.AddVlan(vlan);

            VlanNameTextBox.Text = string.Empty;
            VlanIdTextBox.Text = string.Empty;

            MessageBox.Show($"VLAN '{name}' (ID: {vlanId}) added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnDeleteVlanClicked(object sender, RoutedEventArgs e)
    {
        if (VlanListBox.SelectedItem is Vlan vlan)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete VLAN '{vlan.Name}' (ID: {vlan.VlanId})?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _vlanService.RemoveVlan(vlan);
            }
        }
        else
        {
            MessageBox.Show("Please select a VLAN to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
