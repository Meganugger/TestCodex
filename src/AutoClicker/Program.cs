using AutoClicker.Core;
using AutoClicker.UI;

namespace AutoClicker;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            LogService.Instance.Error($"Unhandled exception: {ex}");
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{ex?.Message}\n\nDetails have been logged.",
                "Auto Clicker - Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        Application.ThreadException += (_, e) =>
        {
            LogService.Instance.Error($"UI thread exception: {e.Exception}");
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nDetails have been logged.",
                "Auto Clicker - Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        LogService.Instance.Info("Application starting.");
        Application.Run(new MainForm());
        LogService.Instance.Info("Application exiting.");
    }
}
