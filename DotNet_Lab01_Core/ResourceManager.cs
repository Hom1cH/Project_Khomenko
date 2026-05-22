namespace DotNet_Lab01_Core;

public class ResourceManager : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    public ResourceManager(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Log file path cannot be empty.", nameof(filePath));

        string? directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

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
