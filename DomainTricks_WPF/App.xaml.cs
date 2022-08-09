using DomainTricks_WPF.Services;
using DomainTricks_WPF.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace DomainTricks_WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly MainWindow appWindow;
    private readonly BackgroundService? backgroundTask;
    
    private readonly ILogger Logger;
    public App()
    {
        // Set up Serilog
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                .WriteTo.File("logs/DomainTricks_Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

        Logger = Log.Logger;
        Log.Information("============= Starting App. =============");

        // Setup and Show the Main Window
        appWindow = new(Logger,new MainViewModel(Logger));
        //appWindow.DataContext = new MainViewModel(Logger);
        appWindow.Show();
    }

    protected override  void OnStartup(StartupEventArgs e)
    {
        Log.Verbose("OnStartup");
        base.OnStartup(e);
    }
    
    protected override  void OnExit(System.Windows.ExitEventArgs e)
    {
        Log.Information($"============= Exiting App. =============");
        base.OnExit(e);
    }


}
