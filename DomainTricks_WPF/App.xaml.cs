using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DomainTricks_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly MainWindow appWindow;

        private readonly ILogger Logger;
        public App()
        {
            // Set up Serilog
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Debug()
                    .WriteTo.File("logs/DomainTricks_Log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

            Logger = Log.Logger;
            Log.Information("Starting App");

            // Setup and Show the Main Window
            appWindow = new(Logger);
            
            appWindow.Show();
        }
    }
}
