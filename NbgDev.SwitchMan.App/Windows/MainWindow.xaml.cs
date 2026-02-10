using System.Windows;

namespace NbgDev.SwitchMan.App.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnSettingsClicked(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Owner = this;
        settingsWindow.ShowDialog();
    }
}
