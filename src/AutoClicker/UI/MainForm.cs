using AutoClicker.Core;

namespace AutoClicker.UI;

/// <summary>
/// MainForm — WinForms chosen over WPF because the original OP Auto Clicker uses
/// classic Win32 controls with a compact fixed layout. WinForms maps 1:1 to that
/// aesthetic, requires fewer dependencies, and keeps the binary small.
/// </summary>
public partial class MainForm : Form
{
    private readonly ClickEngine _engine;
    private readonly HotkeyService _hotkeyService;
    private readonly SettingsService _settingsService;
    private ClickSettings _settings;

    private DateTime _lastHotkeyToggle = DateTime.MinValue;
    private const int HotkeyDebounceMs = 300;

    public MainForm()
    {
        InitializeComponent();

        _settingsService = new SettingsService();
        _settings = _settingsService.Load();
        _engine = new ClickEngine();
        _hotkeyService = new HotkeyService();

        WireEvents();
        ApplySettingsToUI();
        UpdateStatus("Idle");
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RegisterHotkey();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _engine.Stop();
        ReadSettingsFromUI();
        _settingsService.Save(_settings);
        _hotkeyService.Dispose();
        base.OnFormClosing(e);
    }

    // Intercept WM_HOTKEY for the global hotkey.
    protected override void WndProc(ref Message m)
    {
        if (_hotkeyService.IsHotkeyMessage(m))
        {
            if ((DateTime.Now - _lastHotkeyToggle).TotalMilliseconds > HotkeyDebounceMs)
            {
                _lastHotkeyToggle = DateTime.Now;
                ToggleStartStop();
            }
        }
        base.WndProc(ref m);
    }

    private void WireEvents()
    {
        btnStart.Click += (_, _) => DoStart();
        btnStop.Click += (_, _) => DoStop();
        btnHotkeySetting.Click += (_, _) => OpenHotkeySettings();
        btnHelp.Click += (_, _) => OpenHelp();
        btnPickLocation.Click += (_, _) => PickLocation();

        _engine.Started += () =>
        {
            SetRunningState(true);
            UpdateStatus("Running");
        };
        _engine.ClickPerformed += count =>
        {
            UpdateStatus($"Running — {count} click(s)");
        };
        _engine.Stopped += completed =>
        {
            SetRunningState(false);
            UpdateStatus(completed ? "Completed" : "Stopped");
        };

        radioRepeatCount.CheckedChanged += (_, _) => nudRepeatCount.Enabled = radioRepeatCount.Checked;
        radioRepeatUntilStopped.CheckedChanged += (_, _) => nudRepeatCount.Enabled = radioRepeatCount.Checked;
        radioCurrentLocation.CheckedChanged += (_, _) => SetLocationControlsEnabled();
        radioPickLocation.CheckedChanged += (_, _) => SetLocationControlsEnabled();
    }

    private void SetLocationControlsEnabled()
    {
        bool pick = radioPickLocation.Checked;
        btnPickLocation.Enabled = pick;
        nudPickX.Enabled = pick;
        nudPickY.Enabled = pick;
    }

    private void ApplySettingsToUI()
    {
        nudHours.Value = Clamp(_settings.IntervalHours, (int)nudHours.Minimum, (int)nudHours.Maximum);
        nudMinutes.Value = Clamp(_settings.IntervalMinutes, (int)nudMinutes.Minimum, (int)nudMinutes.Maximum);
        nudSeconds.Value = Clamp(_settings.IntervalSeconds, (int)nudSeconds.Minimum, (int)nudSeconds.Maximum);
        nudMilliseconds.Value = Clamp(_settings.IntervalMilliseconds, (int)nudMilliseconds.Minimum, (int)nudMilliseconds.Maximum);

        cmbMouseButton.SelectedIndex = (int)_settings.MouseButton;
        cmbClickType.SelectedIndex = (int)_settings.ClickType;

        if (_settings.RepeatUntilStopped)
            radioRepeatUntilStopped.Checked = true;
        else
            radioRepeatCount.Checked = true;

        nudRepeatCount.Value = Math.Max(1, _settings.RepeatCount);
        nudRepeatCount.Enabled = radioRepeatCount.Checked;

        if (_settings.UseCurrentLocation)
            radioCurrentLocation.Checked = true;
        else
            radioPickLocation.Checked = true;

        nudPickX.Value = Clamp(_settings.PickX, (int)nudPickX.Minimum, (int)nudPickX.Maximum);
        nudPickY.Value = Clamp(_settings.PickY, (int)nudPickY.Minimum, (int)nudPickY.Maximum);

        SetLocationControlsEnabled();
    }

    private void ReadSettingsFromUI()
    {
        _settings.IntervalHours = (int)nudHours.Value;
        _settings.IntervalMinutes = (int)nudMinutes.Value;
        _settings.IntervalSeconds = (int)nudSeconds.Value;
        _settings.IntervalMilliseconds = (int)nudMilliseconds.Value;

        _settings.MouseButton = (MouseButtonType)cmbMouseButton.SelectedIndex;
        _settings.ClickType = (ClickType)cmbClickType.SelectedIndex;

        _settings.RepeatUntilStopped = radioRepeatUntilStopped.Checked;
        _settings.RepeatCount = (int)nudRepeatCount.Value;

        _settings.UseCurrentLocation = radioCurrentLocation.Checked;
        _settings.PickX = (int)nudPickX.Value;
        _settings.PickY = (int)nudPickY.Value;
    }

    private void DoStart()
    {
        ReadSettingsFromUI();

        if (_settings.Interval.TotalMilliseconds < 1)
        {
            MessageBox.Show(
                "Click interval must be at least 1 millisecond.",
                "Invalid Interval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _engine.Start(_settings);
            LogService.Instance.Info("Clicking started.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DoStop()
    {
        _engine.Stop();
        LogService.Instance.Info("Clicking stopped by user.");
    }

    private void ToggleStartStop()
    {
        if (_engine.IsRunning)
            DoStop();
        else
            DoStart();
    }

    private void SetRunningState(bool running)
    {
        btnStart.Enabled = !running;
        btnStop.Enabled = running;

        grpClickInterval.Enabled = !running;
        grpClickOptions.Enabled = !running;
        grpClickRepeat.Enabled = !running;
        grpCursorPosition.Enabled = !running;
    }

    private void UpdateStatus(string status)
    {
        Text = $"OP Auto Clicker 2.1 — {status}";
    }

    private void OpenHotkeySettings()
    {
        using var dlg = new HotkeySettingsDialog(_settings.HotKey, _settings.HotKeyModifiers);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            _settings.HotKey = dlg.SelectedKey;
            _settings.HotKeyModifiers = dlg.SelectedModifiers;
            RegisterHotkey();
            UpdateButtonLabels();
            _settingsService.Save(_settings);
        }
    }

    private void RegisterHotkey()
    {
        _hotkeyService.Register(Handle, _settings.HotKeyModifiers, _settings.HotKey);
        UpdateButtonLabels();
    }

    private void UpdateButtonLabels()
    {
        string label = FormatHotkeyLabel(_settings.HotKeyModifiers, _settings.HotKey);
        btnStart.Text = $"Start ({label})";
        btnStop.Text = $"Stop ({label})";
    }

    private static string FormatHotkeyLabel(uint mod, Keys key)
    {
        var parts = new List<string>();
        if ((mod & NativeMethods.MOD_CONTROL) != 0) parts.Add("Ctrl");
        if ((mod & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
        if ((mod & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
        if ((mod & NativeMethods.MOD_WIN) != 0) parts.Add("Win");
        parts.Add(key.ToString());
        return string.Join("+", parts);
    }

    private void OpenHelp()
    {
        using var dlg = new HelpDialog();
        dlg.ShowDialog(this);
    }

    private void PickLocation()
    {
        using var picker = new LocationPickerForm();
        if (picker.ShowDialog(this) == DialogResult.OK)
        {
            nudPickX.Value = Clamp(picker.PickedX, (int)nudPickX.Minimum, (int)nudPickX.Maximum);
            nudPickY.Value = Clamp(picker.PickedY, (int)nudPickY.Minimum, (int)nudPickY.Maximum);
        }
    }

    private static int Clamp(int value, int min, int max) =>
        Math.Max(min, Math.Min(max, value));
}
