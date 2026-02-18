namespace AutoClicker.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private GroupBox grpClickInterval;
    private NumericUpDown nudHours;
    private NumericUpDown nudMinutes;
    private NumericUpDown nudSeconds;
    private NumericUpDown nudMilliseconds;
    private Label lblHours;
    private Label lblMinutes;
    private Label lblSeconds;
    private Label lblMilliseconds;

    private GroupBox grpClickOptions;
    private Label lblMouseButton;
    private ComboBox cmbMouseButton;
    private Label lblClickType;
    private ComboBox cmbClickType;

    private GroupBox grpClickRepeat;
    private RadioButton radioRepeatCount;
    private NumericUpDown nudRepeatCount;
    private Label lblTimes;
    private RadioButton radioRepeatUntilStopped;

    private GroupBox grpCursorPosition;
    private RadioButton radioCurrentLocation;
    private RadioButton radioPickLocation;
    private Button btnPickLocation;
    private Label lblX;
    private NumericUpDown nudPickX;
    private Label lblY;
    private NumericUpDown nudPickY;

    private Button btnStart;
    private Button btnStop;
    private Button btnHotkeySetting;
    private Button btnHelp;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // --- Form properties ---
        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        ClientSize = new Size(410, 310);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "OP Auto Clicker 2.1";

        // ===== Click interval group =====
        grpClickInterval = new GroupBox
        {
            Text = "Click interval",
            Location = new Point(10, 8),
            Size = new Size(388, 55)
        };

        nudHours = MakeNud(12, 20, 40, 22, 0, 999, 0);
        lblHours = MakeLabel(55, 24, "hours");
        nudMinutes = MakeNud(100, 20, 40, 22, 0, 59, 0);
        lblMinutes = MakeLabel(143, 24, "mins");
        nudSeconds = MakeNud(185, 20, 40, 22, 0, 59, 0);
        lblSeconds = MakeLabel(228, 24, "secs");
        nudMilliseconds = MakeNud(270, 20, 50, 22, 0, 999, 100);
        lblMilliseconds = MakeLabel(323, 24, "milliseconds");

        grpClickInterval.Controls.AddRange(new Control[] {
            nudHours, lblHours, nudMinutes, lblMinutes,
            nudSeconds, lblSeconds, nudMilliseconds, lblMilliseconds
        });

        // ===== Click options group =====
        grpClickOptions = new GroupBox
        {
            Text = "Click options",
            Location = new Point(10, 68),
            Size = new Size(188, 80)
        };

        lblMouseButton = MakeLabel(10, 22, "Mouse button:");
        cmbMouseButton = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(105, 19),
            Size = new Size(72, 23)
        };
        cmbMouseButton.Items.AddRange(new object[] { "Left", "Right", "Middle" });
        cmbMouseButton.SelectedIndex = 0;

        lblClickType = MakeLabel(10, 50, "Click type:");
        cmbClickType = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(105, 47),
            Size = new Size(72, 23)
        };
        cmbClickType.Items.AddRange(new object[] { "Single", "Double" });
        cmbClickType.SelectedIndex = 0;

        grpClickOptions.Controls.AddRange(new Control[] {
            lblMouseButton, cmbMouseButton, lblClickType, cmbClickType
        });

        // ===== Click repeat group =====
        grpClickRepeat = new GroupBox
        {
            Text = "Click repeat",
            Location = new Point(206, 68),
            Size = new Size(192, 80)
        };

        radioRepeatCount = new RadioButton
        {
            Text = "Repeat",
            Location = new Point(10, 20),
            Size = new Size(65, 20),
            AutoSize = false
        };

        nudRepeatCount = MakeNud(78, 19, 55, 22, 1, 999999, 1);

        lblTimes = MakeLabel(138, 22, "times");

        radioRepeatUntilStopped = new RadioButton
        {
            Text = "Repeat until stopped",
            Location = new Point(10, 48),
            Size = new Size(170, 20),
            Checked = true
        };

        grpClickRepeat.Controls.AddRange(new Control[] {
            radioRepeatCount, nudRepeatCount, lblTimes, radioRepeatUntilStopped
        });

        // ===== Cursor position group =====
        grpCursorPosition = new GroupBox
        {
            Text = "Cursor position",
            Location = new Point(10, 153),
            Size = new Size(388, 55)
        };

        radioCurrentLocation = new RadioButton
        {
            Text = "Current location",
            Location = new Point(10, 22),
            Size = new Size(120, 20),
            Checked = true
        };

        radioPickLocation = new RadioButton
        {
            Text = "",
            Location = new Point(135, 22),
            Size = new Size(16, 20)
        };

        btnPickLocation = new Button
        {
            Text = "Pick location",
            Location = new Point(155, 19),
            Size = new Size(90, 24),
            Enabled = false
        };

        lblX = MakeLabel(258, 23, "X");
        nudPickX = MakeNud(272, 20, 50, 22, 0, 9999, 0);
        nudPickX.Enabled = false;

        lblY = MakeLabel(328, 23, "Y");
        nudPickY = MakeNud(342, 20, 50, 22, 0, 9999, 0);
        nudPickY.Enabled = false;

        grpCursorPosition.Controls.AddRange(new Control[] {
            radioCurrentLocation, radioPickLocation, btnPickLocation,
            lblX, nudPickX, lblY, nudPickY
        });

        // ===== Bottom buttons =====
        btnStart = new Button
        {
            Text = "Start (F6)",
            Location = new Point(10, 218),
            Size = new Size(130, 34)
        };

        btnStop = new Button
        {
            Text = "Stop (F6)",
            Location = new Point(210, 218),
            Size = new Size(130, 34),
            Enabled = false
        };

        btnHotkeySetting = new Button
        {
            Text = "Hotkey setting",
            Location = new Point(10, 260),
            Size = new Size(130, 34)
        };

        btnHelp = new Button
        {
            Text = "Help? >>",
            Location = new Point(210, 260),
            Size = new Size(130, 34)
        };

        // ===== Add to form =====
        Controls.AddRange(new Control[] {
            grpClickInterval, grpClickOptions, grpClickRepeat,
            grpCursorPosition,
            btnStart, btnStop, btnHotkeySetting, btnHelp
        });

        ResumeLayout(false);
        PerformLayout();
    }

    private static NumericUpDown MakeNud(int x, int y, int w, int h,
        int min, int max, int value)
    {
        return new NumericUpDown
        {
            Location = new Point(x, y),
            Size = new Size(w, h),
            Minimum = min,
            Maximum = max,
            Value = Math.Max(min, Math.Min(max, value)),
            TextAlign = HorizontalAlignment.Center
        };
    }

    private static Label MakeLabel(int x, int y, string text)
    {
        return new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true
        };
    }
}
