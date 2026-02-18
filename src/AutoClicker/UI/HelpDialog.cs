namespace AutoClicker.UI;

/// <summary>
/// Simple help dialog explaining usage of the Auto Clicker.
/// </summary>
internal sealed class HelpDialog : Form
{
    public HelpDialog()
    {
        Text = "Help — OP Auto Clicker 2.1";
        ClientSize = new Size(420, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var txt = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5f),
            Text = HelpText
        };

        var btnClose = new Button
        {
            Text = "Close",
            Dock = DockStyle.Bottom,
            Height = 34,
            DialogResult = DialogResult.OK
        };

        AcceptButton = btnClose;
        Controls.Add(txt);
        Controls.Add(btnClose);
    }

    private const string HelpText =
@"OP Auto Clicker 2.1
====================

A simple, safe auto-clicker for accessibility, testing, and automation.

CLICK INTERVAL
  Set the delay between each click using hours, minutes, seconds,
  and milliseconds fields. The minimum interval is 1 ms.

CLICK OPTIONS
  Mouse button: choose Left, Right, or Middle.
  Click type: Single sends one click; Double sends two rapid clicks.

CLICK REPEAT
  - Repeat N times: the clicker stops automatically after N clicks.
  - Repeat until stopped: keeps clicking until you press Stop or the
    hotkey.

CURSOR POSITION
  - Current location: clicks wherever the cursor happens to be.
  - Pick location: click the 'Pick location' button, then click
    anywhere on screen to lock the target coordinates. You can also
    type X/Y values directly.

HOTKEY
  Default hotkey is F6 (both Start and Stop toggle).
  Click 'Hotkey setting' to change it. The hotkey works globally,
  even when the app is not focused.

NOTES
  • Settings are saved automatically when you close the app.
  • Logs are written to %LOCALAPPDATA%\AutoClicker\logs\.
  • This tool does NOT require administrator privileges.
  • This is a legitimate accessibility/automation tool.";
}
