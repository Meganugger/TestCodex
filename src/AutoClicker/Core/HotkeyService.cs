namespace AutoClicker.Core;

/// <summary>
/// Registers/unregisters a single system-wide hotkey via RegisterHotKey.
/// The hotkey fires WM_HOTKEY messages to the owning window even when it is
/// not focused, enabling background start/stop.
/// </summary>
internal sealed class HotkeyService : IDisposable
{
    private const int HotkeyId = 9000;
    private IntPtr _hWnd;
    private bool _registered;

    public void Register(IntPtr hWnd, uint modifiers, Keys key)
    {
        Unregister();
        _hWnd = hWnd;
        _registered = NativeMethods.RegisterHotKey(hWnd, HotkeyId, modifiers, (uint)key);
        if (!_registered)
            LogService.Instance.Warn($"RegisterHotKey failed for key {key} (mod {modifiers}).");
        else
            LogService.Instance.Info($"Hotkey registered: mod={modifiers} key={key}");
    }

    public void Unregister()
    {
        if (_registered)
        {
            NativeMethods.UnregisterHotKey(_hWnd, HotkeyId);
            _registered = false;
            LogService.Instance.Info("Hotkey unregistered.");
        }
    }

    public bool IsHotkeyMessage(Message m)
    {
        return m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == HotkeyId;
    }

    public void Dispose() => Unregister();
}
