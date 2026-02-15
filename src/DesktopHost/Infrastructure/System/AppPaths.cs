namespace MovieDeskApp.Infrastructure.System;

internal static class AppPaths
{
    private const string AppFolderName = "MovieDeskApp";

    public static string DataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppFolderName);

    public static string LogFilePath => Path.Combine(DataDirectory, "logs", "app.log");

    public static string WebView2UserDataFolder => Path.Combine(DataDirectory, "WebView2");

    public static string PackagedWebRoot => Path.Combine(AppContext.BaseDirectory, "WebApp");

    public static string PackagedIndexPath => Path.Combine(PackagedWebRoot, "index.html");

    public static string ResolveWebRoot()
    {
        var customPath = Environment.GetEnvironmentVariable("MOVIEDESK_WEBAPP_DIR");
        if (!string.IsNullOrWhiteSpace(customPath) && Directory.Exists(customPath))
        {
            return customPath;
        }

        return PackagedWebRoot;
    }
}
