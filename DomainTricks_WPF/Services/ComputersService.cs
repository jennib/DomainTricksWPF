using DomainTricks_WPF.Models;
using Microsoft.Management.Infrastructure;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services
{
    public class ComputersService
    {

        public  ComputersService(ILogger logger)
        {
            Log.Logger = logger;
        }
      

        public async Task<List<ComputerModel>> GetComputers(string domainPath)
        {
            List<ComputerModel> computers = new ();

            // Get a list of Computers from the Directory.
            ADService adService = new ADService(Log.Logger);
            computers = await adService.GetListOfComputersFromADAsync(domainPath);

            //Get the MMI data for each computer.
            for (int i = 0; i < computers.Count; i++)
            {
                try {
                    ComputerModel newComputerWithMMI = await GetWin32_LogicalDisk(Log.Logger, computers[i]);
                    computers[i] = newComputerWithMMI;
                }
                catch (Exception ex)
                {
                    Log.Error($"Error getting MMI data for {computers[i].Name}.  Error: {ex.Message}");
                }
            }

            return computers;
        }

        private async Task<ComputerModel> GetWin32_LogicalDisk(ILogger logger, ComputerModel computer)
        {
            // Prepare to call MMIService.
            //string computerName = "RELIC-PC";
            AuthenticationModel auth = new("tuttistudios.com", "jennifer", "password");
            // AuthenticationModel auth = new();
            string[] PropertiesArray = { "*" };//{"TotalPhysicalMemory"};
            string ClassName = "Win32_LogicalDisk"; //"Win32_ComputerSystem";
            string FilterName = "DriveType=3";

            MMIService mmiService = new(logger, computer.Name)
            {
                Authentication = auth,
                PropertiesArray = PropertiesArray,
                ClassName = ClassName,
                FilterName = FilterName
            };

            // Call the MMIService .
            try
            {
                await mmiService.Execute();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in TestMMI: {0}", ex.Message);
                throw;
            }

            // Check the Resuylts.
            // The Instances property is of type CimInstance.  
            // It can have multiple Instances and each instance can have multiple Properties.
            if (mmiService.IsError == true)
            {
                Log.Error($"{computer.Name} returned error: {mmiService.ErrorMessage}");
                throw new Exception($"{computer.Name} returned error: {mmiService.ErrorMessage}");
            }
            else
            {
                // Add to the ComputerMOdel.
                computer.InstancesDictionary.Add(ClassName, mmiService.Instances);


                // Log the results.
                Log.Verbose($"{computer.Name} returned: {mmiService.Instances.Count}.");
                foreach (CimInstance instance in mmiService.Instances)
                {
                    Log.Verbose("");

                    // If we asked for only some properties, then we can query for only those properties.
                    // Also check that PropertiesArray does not contain "*" which is the wildcard search, asks for everything.
                    if (PropertiesArray?.Length > 0 && Array.Exists(PropertiesArray, element => element != "*"))
                    {
                        foreach (string property in PropertiesArray)
                        {
                            Log.Verbose($"{property} = {instance.CimInstanceProperties[property].Value}");
                        }
                    }
                    else
                    {
                        // Show us all the properties for the instance.
                        foreach (CimProperty property in instance.CimInstanceProperties)
                        {
                            Log.Verbose($"Name: {property.Name}:{property.Name?.GetType().ToString()} value: {property.Value}:{property.Value?.GetType().ToString()} ");
                        }
                    }
                }
                return computer;
            }
        }
    }
}
