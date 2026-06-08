namespace DotNet_Lab01_Core;

public class ResourceManager : IDisposable
{
    private readonly string _filePath;
    private readonly long _maxFileSizeBytes;
    private StreamWriter _writer;
    private bool _disposed;

    public ResourceManager(string filePath, long maxFileSizeKb = 150)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Log file path cannot be empty.", nameof(filePath));

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        _filePath = filePath;
        _maxFileSizeBytes = maxFileSizeKb * 1024;

        TrimIfNeeded();

        try
        {
            _writer = new StreamWriter(filePath, append: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new InvalidOperationException($"Cannot open log file '{filePath}'.", exception);
        }

        WriteLog("Logger started.");
    }

    public void WriteLog(string message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ResourceManager));

        if (string.IsNullOrWhiteSpace(message))
            message = "Empty log message.";

        _writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
        _writer.Flush();

        TrimIfNeeded();
    }

    private void TrimIfNeeded()
    {
        if (!File.Exists(_filePath))
            return;

        var info = new FileInfo(_filePath);
        if (info.Length < _maxFileSizeBytes)
            return;

        string[] lines = File.ReadAllLines(_filePath);
        int keepFrom = lines.Length / 2;
        string[] kept = lines[keepFrom..];

        _writer?.Dispose();

        File.WriteAllLines(_filePath, kept);
        File.AppendAllText(_filePath,
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [LOG TRIMMED — kept last {kept.Length} lines]{Environment.NewLine}");

        _writer = new StreamWriter(_filePath, append: true);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger stopped.");
        _writer.Dispose();
        _disposed = true;
    }
}
