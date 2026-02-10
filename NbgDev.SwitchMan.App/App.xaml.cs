using System.Windows;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App;

public partial class App : Application
{
    public static VlanService VlanService { get; } = new VlanService();

    public App()
    {
        InitializeComponent();
    }
}
