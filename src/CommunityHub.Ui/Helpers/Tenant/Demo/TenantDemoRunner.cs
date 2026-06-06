using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class TenantDemoRunner
{
    private readonly Button _button;
    private readonly string _startText;
    private readonly string _stopText;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;

    public TenantDemoRunner(Button button, string startText = "▶ Demo", string stopText = "■ Stop")
    {
        _button = button;
        _startText = startText;
        _stopText = stopText;

        SetStoppedState();
    }

    public bool IsRunning => _isRunning;

    public async Task ToggleAsync(Func<CancellationToken, Task> runCycleAsync, Action? restoreState = null)
    {
        if (_isRunning)
        {
            Stop();
            return;
        }

        await StartAsync(runCycleAsync, restoreState);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    public async Task DelayAsync(CancellationToken token, int milliseconds)
    {
        await Task.Delay(milliseconds, token);
    }

    private async Task StartAsync(Func<CancellationToken, Task> runCycleAsync, Action? restoreState)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        _isRunning = true;
        SetRunningState();

        try
        {
            while (!token.IsCancellationRequested)
                await runCycleAsync(token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            restoreState?.Invoke();

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _isRunning = false;
            SetStoppedState();
        }
    }

    private void SetRunningState()
    {
        _button.Content = _stopText;
    }

    private void SetStoppedState()
    {
        _button.Content = _startText;
    }
}