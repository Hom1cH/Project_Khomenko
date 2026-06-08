using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ThemeManager.ApplyTheme(AppTheme.Light);
        MainWindow window = new();
        MainWindow = window;
        window.Show();

        base.OnStartup(e);
    }
}

