using NbgDev.SwitchMan.Pages;

namespace NbgDev.SwitchMan;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage());
    }
}
