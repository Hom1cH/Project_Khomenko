using System.IO;
using System.Text.Json;

namespace WpfApp;

public static class AppSettingsStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static AppSettings Load(string filePath, string defaultExportDirectory)
    {
        if (!File.Exists(filePath))
            return new AppSettings { ExportDirectory = defaultExportDirectory };

        try
        {
            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(stream, JsonOptions);

            if (settings == null || string.IsNullOrWhiteSpace(settings.ExportDirectory))
                return new AppSettings { ExportDirectory = defaultExportDirectory };

            return settings;
        }
        catch
        {
            return new AppSettings { ExportDirectory = defaultExportDirectory };
        }
    }

    public static void Save(AppSettings settings, string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, settings, JsonOptions);
    }
}
