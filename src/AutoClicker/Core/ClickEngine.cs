using System.Runtime.InteropServices;

namespace AutoClicker.Core;

/// <summary>
/// Executes mouse clicks on a background thread using <see cref="NativeMethods.SendInput"/>.
/// Supports finite repeat count or unlimited (until cancelled).
/// Reports progress and completion via events marshalled to the UI thread.
/// </summary>
internal sealed class ClickEngine
{
    public event Action? Started;
    public event Action<long>? ClickPerformed;   // total clicks so far
    public event Action<bool>? Stopped;          // true = completed naturally

    private CancellationTokenSource? _cts;
    private Task? _task;
    private readonly SynchronizationContext _syncContext;

    public bool IsRunning => _task is { IsCompleted: false };

    public ClickEngine()
    {
        _syncContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException("ClickEngine must be created on the UI thread.");
    }

    public void Start(ClickSettings settings)
    {
        if (IsRunning) return;

        var interval = settings.Interval;
        if (interval.TotalMilliseconds < 1)
        {
            throw new ArgumentException("Click interval must be at least 1 ms.");
        }

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var button = settings.MouseButton;
        var clickType = settings.ClickType;
        var repeatForever = settings.RepeatUntilStopped;
        var maxClicks = settings.RepeatCount;
        var useFixed = !settings.UseCurrentLocation;
        int? fixedX = useFixed ? settings.PickX : null;
        int? fixedY = useFixed ? settings.PickY : null;

        Post(() => Started?.Invoke());

        _task = Task.Run(async () =>
        {
            long count = 0;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    PerformClick(button, clickType, fixedX, fixedY);
                    count++;
                    Post(() => ClickPerformed?.Invoke(count));

                    if (!repeatForever && count >= maxClicks)
                    {
                        Post(() => Stopped?.Invoke(true));
                        return;
                    }

                    // High-precision wait: combine Task.Delay for long waits
                    // with a spin-yield tail for sub-16 ms accuracy.
                    if (interval.TotalMilliseconds > 30)
                    {
                        var coarseWait = interval - TimeSpan.FromMilliseconds(15);
                        if (coarseWait.TotalMilliseconds > 0)
                            await Task.Delay(coarseWait, token);
                        SpinWait(interval - coarseWait, token);
                    }
                    else
                    {
                        SpinWait(interval, token);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogService.Instance.Error($"ClickEngine error: {ex}");
            }

            Post(() => Stopped?.Invoke(false));
        }, token);
    }

    public void Stop()
    {
        _cts?.Cancel();
    }

    private static void PerformClick(
        MouseButtonType button, ClickType clickType,
        int? x, int? y)
    {
        var inputs = NativeMethods.BuildClickInputs(button, x, y);
        int size = Marshal.SizeOf<NativeMethods.INPUT>();

        // Single click = down + up
        NativeMethods.SendInput((uint)inputs.Length, inputs, size);

        if (clickType == ClickType.Double)
        {
            // Double click = repeat down + up immediately
            NativeMethods.SendInput((uint)inputs.Length, inputs, size);
        }
    }

    private static void SpinWait(TimeSpan duration, CancellationToken token)
    {
        var end = DateTime.UtcNow + duration;
        while (DateTime.UtcNow < end)
        {
            if (token.IsCancellationRequested) return;
            Thread.SpinWait(10);
            Thread.Yield();
        }
    }

    private void Post(Action action)
    {
        _syncContext.Post(_ => action(), null);
    }
}
