using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
namespace WpfApp;

public partial class SettingsWindow : Window
{
    private readonly List<GamblingApp> _gamblingApps;

    public SettingsWindow(string exportDirectory, AppTheme theme, bool reminderEnabled, List<GamblingApp> gamblingApps)
    {
        InitializeComponent();

        ExportPathTextBox.Text = exportDirectory;
        DarkThemeToggle.IsChecked = theme == AppTheme.Dark;
        ReminderToggle.IsChecked = reminderEnabled;

        _gamblingApps = new List<GamblingApp>(gamblingApps);
        RefreshGamblingList();
    }

    public string ExportDirectory => ExportPathTextBox.Text.Trim();
    public AppTheme SelectedTheme => DarkThemeToggle.IsChecked == true ? AppTheme.Dark : AppTheme.Light;
    public bool ReminderEnabled => ReminderToggle.IsChecked == true;
    public List<GamblingApp> GamblingApps => _gamblingApps;

    public event Action<SettingsWindow>? ApplyRequested;

    private void RefreshGamblingList()
    {
        GamblingAppsList.Items.Clear();
        foreach (var app in _gamblingApps)
            GamblingAppsList.Items.Add($"{app.Name}  Ч  {app.ExePath}");
    }

    private void AddGamblingApp_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select executable",
            Filter = "Executable files (*.exe)|*.exe"
        };

        if (dialog.ShowDialog(this) != true) return;

        string exePath = dialog.FileName;
        string name = Path.GetFileNameWithoutExtension(exePath);

        // якщо назва вже Ї Ч не дублюЇмо
        if (_gamblingApps.Any(a => a.ExePath == exePath))
        {
            MessageBox.Show("÷€ програма вже Ї у списку.", "Gambling Apps",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _gamblingApps.Add(new GamblingApp { Name = name, ExePath = exePath });
        RefreshGamblingList();
    }

    private void RemoveGamblingApp_Click(object sender, RoutedEventArgs e)
    {
        int index = GamblingAppsList.SelectedIndex;
        if (index < 0)
        {
            MessageBox.Show("¬ибер≥ть програму з≥ списку.", "Gambling Apps",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _gamblingApps.RemoveAt(index);
        RefreshGamblingList();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        using System.Windows.Forms.FolderBrowserDialog dialog = new()
        {
            Description = "Select folder for JSON and XML files",
            SelectedPath = ExportPathTextBox.Text
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            ExportPathTextBox.Text = dialog.SelectedPath;
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateSettings()) return;
        ApplyRequested?.Invoke(this);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private bool ValidateSettings()
    {
        if (!string.IsNullOrWhiteSpace(ExportPathTextBox.Text))
            return true;

        MessageBox.Show("Select folder for JSON and XML files.", "Settings",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }
}
