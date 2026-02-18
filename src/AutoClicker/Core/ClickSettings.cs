using System.Text.Json.Serialization;

namespace AutoClicker.Core;

/// <summary>
/// Serializable settings model. Persisted as JSON in %LOCALAPPDATA%.
/// </summary>
internal sealed class ClickSettings
{
    public int IntervalHours { get; set; }
    public int IntervalMinutes { get; set; }
    public int IntervalSeconds { get; set; }
    public int IntervalMilliseconds { get; set; } = 100;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MouseButtonType MouseButton { get; set; } = MouseButtonType.Left;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ClickType ClickType { get; set; } = ClickType.Single;

    public bool RepeatUntilStopped { get; set; } = true;
    public int RepeatCount { get; set; } = 1;

    public bool UseCurrentLocation { get; set; } = true;
    public int PickX { get; set; }
    public int PickY { get; set; }

    public Keys HotKey { get; set; } = Keys.F6;
    public uint HotKeyModifiers { get; set; } = NativeMethods.MOD_NONE;

    [JsonIgnore]
    public TimeSpan Interval =>
        new(0, IntervalHours, IntervalMinutes, IntervalSeconds, IntervalMilliseconds);
}
