# MovieDesk (Windows Desktop Wrapper)

Production-ready Windows desktop wrapper for your existing web UI/logic (`index.html` as source of truth).

## Why this stack

- **WPF + WebView2 (.NET 8)** for native Windows shell behavior with a modern Chromium rendering engine.
- Fast startup, low complexity, and easy maintenance compared to Electron for this use case.
- Security controls are centralized in `MainWindow.xaml.cs` (navigation allowlist, new-window policy).

## Project structure

```text
src/
  MovieDeskApp.sln
  DesktopHost/
    App.xaml
    MainWindow.xaml
    MainWindow.xaml.cs
    DesktopHost.csproj
    Infrastructure/
      Logging/Logger.cs
      System/AppPaths.cs
    WebApp/
      index.html
      styles.css
      app.js
```


## Use your existing website as source of truth (not a demo replacement)

1. Replace files under `src/DesktopHost/WebApp/` with your real website assets (`index.html`, CSS, JS, images).
2. Keep relative paths (e.g. `./styles.css`, `./app.js`) so they resolve inside the packaged folder.
3. If you want to run with an external web folder during development, set:

```bash
set MOVIEDESK_WEBAPP_DIR=C:\path\to\your\site
```

The host maps that folder to `https://app.moviedesk.local/` via WebView2 virtual host mapping.

## Run in development

```bash
dotnet restore src/MovieDeskApp.sln
dotnet run --project src/DesktopHost/DesktopHost.csproj -c Debug
```

## Build release executable

```bash
dotnet publish src/DesktopHost/DesktopHost.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

Output: `src/DesktopHost/bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/MovieDeskApp.exe`

## Installer options

### Option A (recommended for internal distribution): MSIX

1. Create a **Windows Application Packaging Project** in Visual Studio.
2. Reference `DesktopHost` as the entry app.
3. Configure identity, publisher, and signing certificate.
4. Build `.msix` package.

### Option B (simple external installer): Inno Setup

Use a script to package the published folder and create `MovieDesk-Setup.exe`.

## Hardening notes

- Navigation policy only permits local file content and selected trusted hosts (YouTube + Drive).
- `window.open` requests are redirected to the default OS browser.
- Logging is written to `%LOCALAPPDATA%\MovieDeskApp\logs\app.log`.
- DevTools menu is shown only in Debug builds.

## Polish checklist

- [ ] Add custom app icon + branding assets.
- [ ] Add keyboard shortcuts (Ctrl+F focus search, Ctrl+R reload).
- [ ] Add reduced-motion mode and larger text mode.
- [ ] Add analytics/error telemetry sink.
- [ ] Validate 125%/150% Windows DPI layout.
- [ ] Add CI pipeline for build + signing.
