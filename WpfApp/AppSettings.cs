namespace WpfApp;

public sealed class AppSettings
{
    public string ExportDirectory { get; set; } = string.Empty;
    public AppTheme Theme { get; set; } = AppTheme.Light;
    public bool ReminderEnabled { get; set; } = true;
    public List<GamblingApp> GamblingApps { get; set; } = new();
}
