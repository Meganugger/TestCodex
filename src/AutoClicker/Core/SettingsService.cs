using System.Text.Json;

namespace AutoClicker.Core;

/// <summary>
/// Persists <see cref="ClickSettings"/> to a JSON file in %LOCALAPPDATA%\AutoClicker\.
/// Loads defaults gracefully if the file is missing or corrupt.
/// </summary>
internal sealed class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AutoClicker", "settings.json");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true
    };

    public ClickSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<ClickSettings>(json, JsonOpts);
                if (settings != null)
                {
                    LogService.Instance.Info("Settings loaded.");
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            LogService.Instance.Warn($"Failed to load settings: {ex.Message}");
        }

        LogService.Instance.Info("Using default settings.");
        return new ClickSettings();
    }

    public void Save(ClickSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(settings, JsonOpts);
            File.WriteAllText(SettingsPath, json);
            LogService.Instance.Info("Settings saved.");
        }
        catch (Exception ex)
        {
            LogService.Instance.Warn($"Failed to save settings: {ex.Message}");
        }
    }
}
