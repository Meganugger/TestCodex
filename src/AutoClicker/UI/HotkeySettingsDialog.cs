using AutoClicker.Core;

namespace AutoClicker.UI;

/// <summary>
/// Dialog that captures a new global hotkey combination.
/// User presses the desired key combo and clicks OK.
/// </summary>
internal sealed class HotkeySettingsDialog : Form
{
    private Label lblInstruction = null!;
    private TextBox txtHotkey = null!;
    private Button btnOk = null!;
    private Button btnCancel = null!;
    private Button btnReset = null!;

    public Keys SelectedKey { get; private set; }
    public uint SelectedModifiers { get; private set; }

    private Keys _capturedKey;
    private uint _capturedMod;

    public HotkeySettingsDialog(Keys currentKey, uint currentModifiers)
    {
        SelectedKey = currentKey;
        SelectedModifiers = currentModifiers;
        _capturedKey = currentKey;
        _capturedMod = currentModifiers;
        InitializeComponent();
        UpdateDisplay();
    }

    private void InitializeComponent()
    {
        Text = "Hotkey Setting";
        ClientSize = new Size(300, 130);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;

        lblInstruction = new Label
        {
            Text = "Press the desired hotkey combination:",
            Location = new Point(12, 12),
            AutoSize = true
        };

        txtHotkey = new TextBox
        {
            Location = new Point(12, 36),
            Size = new Size(270, 23),
            ReadOnly = true,
            TextAlign = HorizontalAlignment.Center,
            Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
        };

        btnReset = new Button
        {
            Text = "Reset (F6)",
            Location = new Point(12, 72),
            Size = new Size(80, 30)
        };
        btnReset.Click += (_, _) =>
        {
            _capturedKey = Keys.F6;
            _capturedMod = NativeMethods.MOD_NONE;
            UpdateDisplay();
        };

        btnOk = new Button
        {
            Text = "OK",
            Location = new Point(120, 72),
            Size = new Size(75, 30),
            DialogResult = DialogResult.OK
        };
        btnOk.Click += (_, _) =>
        {
            SelectedKey = _capturedKey;
            SelectedModifiers = _capturedMod;
        };

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(205, 72),
            Size = new Size(75, 30),
            DialogResult = DialogResult.Cancel
        };

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        Controls.AddRange(new Control[] { lblInstruction, txtHotkey, btnReset, btnOk, btnCancel });

        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        e.SuppressKeyPress = true;

        // Ignore standalone modifier presses.
        if (e.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)
            return;

        _capturedMod = NativeMethods.MOD_NONE;
        if (e.Control) _capturedMod |= NativeMethods.MOD_CONTROL;
        if (e.Alt)     _capturedMod |= NativeMethods.MOD_ALT;
        if (e.Shift)   _capturedMod |= NativeMethods.MOD_SHIFT;

        _capturedKey = e.KeyCode;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        var parts = new List<string>();
        if ((_capturedMod & NativeMethods.MOD_CONTROL) != 0) parts.Add("Ctrl");
        if ((_capturedMod & NativeMethods.MOD_ALT) != 0) parts.Add("Alt");
        if ((_capturedMod & NativeMethods.MOD_SHIFT) != 0) parts.Add("Shift");
        parts.Add(_capturedKey.ToString());
        txtHotkey.Text = string.Join(" + ", parts);
    }
}
