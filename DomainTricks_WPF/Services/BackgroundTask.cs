using DomainTricks_WPF.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services;

public class BackgroundTask
{

    // Call Example
    // var task = new BackgroundTask(TimeSpan.FromMilliseconds(1000));
    // task.Start();
    // await task.StopAsync();

    private Task? _timerTask;
    private PeriodicTimer? _timer;
    private  CancellationTokenSource _tokenSource = new();
    private MainViewModel _mainViewModel;
    
    public BackgroundTask(ILogger logger, MainViewModel mainViewModel)
    {
        Log.Logger = logger;
        _mainViewModel = mainViewModel;
       // _timer = new(interval;
    }

    public void Start(TimeSpan interval)
    {
        _timer = new (interval);
        _timerTask = DoWorkAsync();
    }

    public async Task DoWorkAsync()
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(_tokenSource.Token) && !_tokenSource.IsCancellationRequested)
            {
                Log.Information($"Timer tick " + DateTime.Now.ToString("O"));
                await _mainViewModel.RefreshComputers();
            }
        }
        catch (OperationCanceledException)
        {
            // Eat this exception.  It is expected.
        }
        catch (Exception ex) 
        {
            Log.Error(ex, "Exception in DoWorkAsnyc: {0}", ex.Message);
        }
    }
    public async Task StopAsync()
    {
        if (_timerTask != null)
        {
            _tokenSource.Cancel();
            await _timerTask;
            _tokenSource.Dispose();
            Log.Information("PeriodidTimer task was canceled");
        }
    }
}