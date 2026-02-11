using Microsoft.AspNetCore.Components;
using NbgDev.SwitchMan.App.Models;

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
}
