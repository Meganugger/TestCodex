using AutoClicker.Core;

namespace AutoClicker.UI;

/// <summary>
/// Full-screen transparent overlay that lets the user click anywhere to pick
/// screen coordinates. Displays crosshair cursor and shows live coordinates.
/// Pressing Escape cancels.
/// </summary>
internal sealed class LocationPickerForm : Form
{
    public int PickedX { get; private set; }
    public int PickedY { get; private set; }

    private readonly Label _coordLabel;

    public LocationPickerForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;
        Cursor = Cursors.Cross;
        BackColor = Color.White;
        Opacity = 0.25;
        DoubleBuffered = true;

        _coordLabel = new Label
        {
            AutoSize = true,
            Font = new Font("Consolas", 14, FontStyle.Bold),
            ForeColor = Color.Black,
            BackColor = Color.FromArgb(200, 255, 255, 200),
            Padding = new Padding(6),
            Location = new Point(20, 20),
            Text = "Click anywhere to pick location. Press Esc to cancel."
        };
        Controls.Add(_coordLabel);

        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        };

        MouseMove += (_, e) =>
        {
            // Convert form-relative coordinates to screen coordinates.
            var screen = PointToScreen(e.Location);
            _coordLabel.Text = $"X: {screen.X}  Y: {screen.Y}   (click to select, Esc to cancel)";
        };

        MouseClick += (_, e) =>
        {
            var screen = PointToScreen(e.Location);
            PickedX = screen.X;
            PickedY = screen.Y;
            LogService.Instance.Info($"Location picked: ({PickedX}, {PickedY})");
            DialogResult = DialogResult.OK;
            Close();
        };
    }
}
