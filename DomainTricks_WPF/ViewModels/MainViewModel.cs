using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(ILogger logger)
        {
            Log.Logger = logger;
            Log.Information("MainViewModel start.");
        }
    }
}
