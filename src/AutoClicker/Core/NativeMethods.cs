using System.Runtime.InteropServices;

namespace AutoClicker.Core;

/// <summary>
/// P/Invoke declarations for Windows input simulation and global hotkeys.
/// All structures and constants follow the Win32 API documentation.
/// </summary>
internal static class NativeMethods
{
    // --- SendInput: simulates mouse (and keyboard) events at the driver level ---

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    internal static extern int GetSystemMetrics(int nIndex);

    // Virtual screen dimensions used to normalize absolute coordinates for SendInput.
    internal const int SM_CXSCREEN = 0;
    internal const int SM_CYSCREEN = 1;

    // INPUT structure type discriminator
    internal const uint INPUT_MOUSE = 0;

    // MOUSEEVENTF flags
    internal const uint MOUSEEVENTF_MOVE       = 0x0001;
    internal const uint MOUSEEVENTF_LEFTDOWN   = 0x0002;
    internal const uint MOUSEEVENTF_LEFTUP     = 0x0004;
    internal const uint MOUSEEVENTF_RIGHTDOWN  = 0x0008;
    internal const uint MOUSEEVENTF_RIGHTUP    = 0x0010;
    internal const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    internal const uint MOUSEEVENTF_MIDDLEUP   = 0x0040;
    internal const uint MOUSEEVENTF_ABSOLUTE   = 0x8000;

    // --- Global hotkey registration (works even when app is not focused) ---

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    // Hotkey modifier flags
    internal const uint MOD_NONE    = 0x0000;
    internal const uint MOD_ALT     = 0x0001;
    internal const uint MOD_CONTROL = 0x0002;
    internal const uint MOD_SHIFT   = 0x0004;
    internal const uint MOD_WIN     = 0x0008;

    // WM_HOTKEY message constant
    internal const int WM_HOTKEY = 0x0312;

    // --- Cursor position ---

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetCursorPos(int x, int y);

    // --- Structures ---

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        public uint Type;
        public MOUSEINPUT Mi;
    }

    // Padded to match the size of the C union (MOUSEINPUT is the largest member).
    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        public int Dx;
        public int Dy;
        public uint MouseData;
        public uint DwFlags;
        public uint Time;
        public IntPtr DwExtraInfo;
    }

    /// <summary>
    /// Build a pair of INPUT structures for a mouse button down + up event.
    /// If <paramref name="absoluteX"/> and <paramref name="absoluteY"/> are provided
    /// the click is sent to that screen coordinate; otherwise it fires at the current
    /// cursor position.
    /// </summary>
    internal static INPUT[] BuildClickInputs(
        MouseButtonType button,
        int? absoluteX = null,
        int? absoluteY = null)
    {
        uint downFlag, upFlag;
        switch (button)
        {
            case MouseButtonType.Right:
                downFlag = MOUSEEVENTF_RIGHTDOWN;
                upFlag   = MOUSEEVENTF_RIGHTUP;
                break;
            case MouseButtonType.Middle:
                downFlag = MOUSEEVENTF_MIDDLEDOWN;
                upFlag   = MOUSEEVENTF_MIDDLEUP;
                break;
            default:
                downFlag = MOUSEEVENTF_LEFTDOWN;
                upFlag   = MOUSEEVENTF_LEFTUP;
                break;
        }

        uint commonFlags = 0;
        int dx = 0, dy = 0;

        if (absoluteX.HasValue && absoluteY.HasValue)
        {
            // Normalize to 0–65535 range required by MOUSEEVENTF_ABSOLUTE.
            int screenW = GetSystemMetrics(SM_CXSCREEN);
            int screenH = GetSystemMetrics(SM_CYSCREEN);
            dx = (int)((absoluteX.Value * 65535.0) / (screenW - 1));
            dy = (int)((absoluteY.Value * 65535.0) / (screenH - 1));
            commonFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
        }

        var down = new INPUT
        {
            Type = INPUT_MOUSE,
            Mi = new MOUSEINPUT
            {
                Dx = dx, Dy = dy,
                DwFlags = commonFlags | downFlag,
                Time = 0, DwExtraInfo = IntPtr.Zero
            }
        };

        var up = new INPUT
        {
            Type = INPUT_MOUSE,
            Mi = new MOUSEINPUT
            {
                Dx = dx, Dy = dy,
                DwFlags = commonFlags | upFlag,
                Time = 0, DwExtraInfo = IntPtr.Zero
            }
        };

        return [down, up];
    }
}

internal enum MouseButtonType
{
    Left,
    Right,
    Middle
}

internal enum ClickType
{
    Single,
    Double
}
