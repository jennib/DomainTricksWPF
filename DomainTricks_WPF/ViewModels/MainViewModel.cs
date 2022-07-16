using DomainTricks_WPF.Models;
using DomainTricks_WPF.Services;
using Microsoft.Management.Infrastructure;
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
            string computerName = "RELIC-PC";
           await mmiService.GetMMI(computerName);
            if (mmiService.IsError == true)
            {
                Log.Information($"{computerName} returned error: {mmiService.ErrorMessage}");
            }
            else
            {
                Log.Information($"{computerName} returned: {mmiService.Instances.Count()}");
            }
           
        }
    }
}
