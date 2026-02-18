# OP Auto Clicker 2.1

A production-grade Windows auto-clicker for **accessibility, testing, and automation**.  
Replicates the classic OP Auto Clicker UI and behavior using C# / .NET 8 WinForms.

> **Disclaimer:** This tool is intended for legitimate automation, accessibility, and
> testing purposes only. It contains no malware or hidden behavior.

---

## Features

| Feature | Details |
|---|---|
| **Click interval** | Hours / minutes / seconds / milliseconds with precise timing |
| **Mouse button** | Left, Right, Middle |
| **Click type** | Single or Double click |
| **Repeat mode** | Fixed count or unlimited (until stopped) |
| **Cursor position** | Current location or pick a fixed screen coordinate |
| **Global hotkey** | Works even when the app is not focused (default: F6) |
| **Settings persistence** | Auto-saved to `%LOCALAPPDATA%\AutoClicker\settings.json` |
| **Logging** | Rolling daily logs in `%LOCALAPPDATA%\AutoClicker\logs\` |
| **DPI-aware** | PerMonitorV2 scaling for crisp rendering at 125–200% |
| **No admin required** | Runs as standard user |

## Architecture

```
src/AutoClicker/
  Core/
    NativeMethods.cs      # P/Invoke: SendInput, RegisterHotKey, GetCursorPos
    ClickEngine.cs        # Background click loop with CancellationToken
    HotkeyService.cs      # Global hotkey registration/unregistration
    SettingsService.cs     # JSON persistence for user settings
    LogService.cs          # Thread-safe rolling daily file logger
    ClickSettings.cs       # Settings data model
  UI/
    MainForm.cs            # Main window logic (event wiring only)
    MainForm.Designer.cs   # Control layout matching OP Auto Clicker 2.1
    HotkeySettingsDialog.cs# Hotkey capture dialog
    HelpDialog.cs          # Usage help
    LocationPickerForm.cs  # Full-screen crosshair coordinate picker
  Program.cs               # Entry point with global exception handling
  Properties/
    app.manifest           # DPI awareness + asInvoker execution level
```

**Design principle:** All Win32 interop and click logic lives in `Core/`. The UI layer
(`MainForm.cs`) contains only control event wiring — no business logic in code-behind.

## Requirements

- **Windows 10 or 11**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for building)
- .NET 8 Desktop Runtime (for running pre-built binaries)

## Build & Run

```bash
# Restore + build
dotnet build src/AutoClicker.sln -c Release

# Run directly
dotnet run --project src/AutoClicker/AutoClicker.csproj -c Release
```

## Publish a standalone executable

```bash
dotnet publish src/AutoClicker/AutoClicker.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true
```

Output: `src/AutoClicker/bin/Release/net8.0-windows/win-x64/publish/AutoClicker.exe`

To add a custom icon, place an `.ico` file in `Resources/` and add to the `.csproj`:

```xml
<PropertyGroup>
  <ApplicationIcon>Resources\app.ico</ApplicationIcon>
</PropertyGroup>
```

## Usage

1. **Set interval** — enter hours, minutes, seconds, and milliseconds.
2. **Choose options** — select mouse button (Left/Right/Middle) and click type (Single/Double).
3. **Set repeat mode** — fixed count or repeat until stopped.
4. **Set cursor target** — current mouse position or pick a fixed screen coordinate.
5. **Press Start (F6)** — clicking begins. Press **F6 again** (or click Stop) to halt.
6. **Change hotkey** — click "Hotkey setting" to assign a different key combo.

## License

MIT — free to use, modify, and distribute.
