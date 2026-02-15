using System.Text;
using MovieDeskApp.Infrastructure.System;

namespace MovieDeskApp.Infrastructure.Logging;

internal static class Logger
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);

    public static async Task InfoAsync(string message) => await WriteAsync("INFO", message);
    public static async Task WarnAsync(string message) => await WriteAsync("WARN", message);
    public static async Task ErrorAsync(string message, Exception? ex = null)
    {
        var details = ex is null ? message : $"{message}{Environment.NewLine}{ex}";
        await WriteAsync("ERROR", details);
    }

    private static async Task WriteAsync(string level, string message)
    {
        var logDirectory = Path.GetDirectoryName(AppPaths.LogFilePath);
        if (!string.IsNullOrWhiteSpace(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        var line = $"[{DateTimeOffset.Now:O}] [{level}] {message}{Environment.NewLine}";
        var bytes = Encoding.UTF8.GetBytes(line);

        await FileLock.WaitAsync();
        try
        {
            await using var stream = new FileStream(AppPaths.LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            await stream.WriteAsync(bytes);
        }
        finally
        {
            FileLock.Release();
        }
    }
}
