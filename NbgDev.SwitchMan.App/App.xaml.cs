using NbgDev.SwitchMan.App.Pages;

namespace NbgDev.SwitchMan.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage());
    }
}
