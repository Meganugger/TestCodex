namespace AutoClicker.Core;

/// <summary>
/// Lightweight rolling daily log written to %LOCALAPPDATA%\AutoClicker\logs\.
/// Thread-safe via lock. Auto-creates directory on first write.
/// </summary>
internal sealed class LogService
{
    public static readonly LogService Instance = new();

    private readonly string _logDir;
    private readonly object _lock = new();

    private LogService()
    {
        _logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoClicker", "logs");
    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message) => Write("ERROR", message);

    private void Write(string level, string message)
    {
        try
        {
            lock (_lock)
            {
                Directory.CreateDirectory(_logDir);
                var path = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
                var line = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(path, line);
            }
        }
        catch
        {
            // Logging must never crash the app.
        }
    }
}
