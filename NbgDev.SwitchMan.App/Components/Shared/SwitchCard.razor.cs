using Microsoft.AspNetCore.Components;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.Switches.Contract;
using NbgDev.SwitchMan.Switches.Contract.Models;

namespace NbgDev.SwitchMan.App.Components.Shared;

public partial class SwitchCard
{
    [Parameter]
    public Switch Switch { get; set; } = null!;

    [Parameter]
    public EventCallback OnMoveUp { get; set; }

    [Parameter]
    public EventCallback OnMoveDown { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    [Inject]
    private ISwitchAccessService SwitchAccessService { get; set; } = null!;

    [Inject]
    private ILogger<SwitchCard> Logger { get; set; } = null!;

    private IEnumerable<PortInfo>? _ports;
    private bool _isLoading = true;
    private string? _loadError;

    protected override async Task OnInitializedAsync()
    {
        await LoadPortsAsync();
    }

    private async Task LoadPortsAsync()
    {
        _isLoading = true;
        _loadError = null;
        try
        {
            _ports = await SwitchAccessService.GetPortVlansAsync(Switch.IpAddress);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load port information for switch {SwitchName} at {IpAddress}.", Switch.Name, Switch.IpAddress);
            _loadError = "Failed to load port information. Please try again.";
        }
        finally
        {
            _isLoading = false;
        }
    }
}
