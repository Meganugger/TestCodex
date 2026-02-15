using System.Diagnostics;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using MovieDeskApp.Infrastructure.Logging;
using MovieDeskApp.Infrastructure.System;

namespace MovieDeskApp;

public partial class MainWindow : Window
{
    private const string LocalHostName = "app.moviedesk.local";

    private static readonly string[] AllowedHosts =
    [
        LocalHostName,
        "www.youtube.com",
        "youtube.com",
        "www.youtube-nocookie.com",
        "drive.google.com"
    ];

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
#if !DEBUG
        DevToolsMenuItem.Visibility = Visibility.Collapsed;
#endif
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.DataDirectory);

            var webRoot = AppPaths.ResolveWebRoot();
            var indexPath = Path.Combine(webRoot, "index.html");
            if (!File.Exists(indexPath))
            {
                throw new FileNotFoundException("Web app entry point not found.", indexPath);
            }

            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: AppPaths.WebView2UserDataFolder);
            await AppWebView.EnsureCoreWebView2Async(environment);

            AppWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
#if DEBUG
            AppWebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
#else
            AppWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif
            AppWebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            AppWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;

            AppWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                LocalHostName,
                webRoot,
                CoreWebView2HostResourceAccessKind.DenyCors);

            AppWebView.CoreWebView2.NavigationStarting += OnNavigationStarting;
            AppWebView.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
            AppWebView.CoreWebView2.ProcessFailed += OnProcessFailed;

            AppWebView.Source = new Uri($"https://{LocalHostName}/index.html");
            await Logger.InfoAsync($"Desktop host initialized successfully. Web root: {webRoot}");
        }
        catch (Exception ex)
        {
            await Logger.ErrorAsync("Failed to initialize desktop host.", ex);
            MessageBox.Show(
                "MovieDesk failed to start. Check logs in %LOCALAPPDATA%\\MovieDeskApp\\logs.",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri))
        {
            return;
        }

        if (uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) &&
            AllowedHosts.Any(host => uri.Host.Equals(host, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        e.Cancel = true;
        OpenExternalLink(uri.ToString());
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        OpenExternalLink(e.Uri);
    }

    private async void OnProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
    {
        await Logger.ErrorAsync($"WebView2 process failed: {e.ProcessFailedKind}");
        MessageBox.Show("The embedded browser process failed. Please reload the app.", "WebView Error",
            MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void OnReloadClick(object sender, RoutedEventArgs e)
    {
        AppWebView.Reload();
    }

    private void OnDevToolsClick(object sender, RoutedEventArgs e)
    {
#if DEBUG
        AppWebView.CoreWebView2?.OpenDevToolsWindow();
#endif
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private static void OpenExternalLink(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // no-op: fallback is graceful failure
        }
    }
}
