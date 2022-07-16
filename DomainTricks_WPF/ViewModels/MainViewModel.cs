using DomainTricks_WPF.Models;
using DomainTricks_WPF.Services;
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
        public  MainViewModel(ILogger logger)
        {
            Log.Logger = logger;
            Log.Information("MainViewModel start.");
            ComputerModel computer = new(logger, Guid.NewGuid());
            MMIService mmiService = new(logger);
            TestMMI(mmiService);
        }


        // Test the Microsoft Management Infrastructure call
       async void TestMMI(MMIService mmiService)
        {
            await mmiService.GetMMI("RELIC");
        }
    }
}
