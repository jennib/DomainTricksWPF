using DomainTricks_WPF.Models;
using Microsoft.Management.Infrastructure;
using Serilog;
using Serilog.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services
{
    public class ComputersService
    {
        public ComputersService(ILogger logger)
        {
            Log.Logger = logger;
        }

        public async Task<List<ComputerModel>> GetComputers(string domainPath)
        {
            List<ComputerModel> computers = new();
            
            // Setup the authentication credentials
           // AuthenticationModel auth = new("", "", "",true);
           AuthenticationModel auth = new("tuttistudios.com", "jennifer", "password",false);

            // Get a list of Computers from the Directory.
            ADService adService = new ADService(Log.Logger);
            computers = await adService.GetListOfComputersFromADAsync(domainPath);

            //await Task.Run(() =>
            //{
            //    Parallel.ForEach<ComputerModel>(computers, (computer) =>
            //    {
            foreach (ComputerModel computer in computers)
            {
                Log.Information($"Testing Computer {computer.Name}");
                MMIService mmiService = new(Log.Logger, computer.Name);
                bool isTestSuccessful = mmiService.TestConnection().Result;
                Log.Information($"{computer.Name} tests: {isTestSuccessful}");
                if (isTestSuccessful)
                {
                    computer.DateLastSeen = DateTime.Now;
                }
            }
            //    });
            //});

            computers = await GetComputers_Win32_LogicalDisks(Log.Logger, computers, auth);

            computers = await GetComputers_Win32_ComputerSystem(Log.Logger, computers, auth);

            return computers;
        }
        private async Task<List<ComputerModel>> GetComputers_Win32_ComputerSystem(ILogger logger, List<ComputerModel> computers, AuthenticationModel auth)
        {
            string[] PropertiesArray = { "*" };//{"TotalPhysicalMemory"};
            string ClassName = "Win32_ComputerSystem"; //"Win32_ComputerSystem";
            string FilterName = "";

            List<ComputerModel> newComputers = new();

            //Get the MMI data for each computer.
            newComputers = await GetListOfComputersWithInstances(Log.Logger, computers, PropertiesArray, ClassName, FilterName, auth);

            return newComputers;
        }
        private async Task<List<ComputerModel>> GetComputers_Win32_LogicalDisks(ILogger logger, List<ComputerModel> computers, AuthenticationModel auth)
        {
            string[] PropertiesArray = { "*" };//{"TotalPhysicalMemory"};
            string ClassName = "Win32_LogicalDisk"; //"Win32_ComputerSystem";
            string FilterName = "DriveType=3";

            List<ComputerModel> newComputers = new();

            //Get the MMI data for each computer.
            newComputers = await GetListOfComputersWithInstances(Log.Logger, computers, PropertiesArray, ClassName, FilterName, auth);

            // Add the Win32_LogicalDisk data to the ComputerModel ListOfWin32_LogicalDisk.
            foreach (ComputerModel computer in newComputers)
            {
                List<DriveModel> myDrives = new();

                foreach (var kvp in computer.InstancesDictionary.Values)
                {
                    foreach (CimInstance cimInstance in kvp)
                    {
                        Log.Information($"{cimInstance.CimInstanceProperties.Count} drives found for {computer.Name}");
                        DriveModel drive = new();
                        drive.DeviceID = cimInstance.CimInstanceProperties["DeviceID"].Value.ToString();
                        drive.FreeSpace = (UInt64)cimInstance.CimInstanceProperties["FreeSpace"].Value;
                        drive.Size = (UInt64)cimInstance.CimInstanceProperties["Size"].Value;
                        myDrives.Add(drive);
                    }
                }
                computer.ListOfWin32_LogicalDisk = myDrives;
            }

            return newComputers;
        }
        private async Task<List<ComputerModel>> GetListOfComputersWithInstances(ILogger logger,
        List<ComputerModel> computers,
        string[] propertiesArray,
        string className,
        string filterName,
        AuthenticationModel auth)
        {
            List<ComputerModel> newComputers = new();

            //Get the MMI data for each computer.
            await Task.Run(() =>
            {
                Parallel.ForEach<ComputerModel>(computers, (computer) =>
                {
                    try
                    {
                        ComputerModel newComputerWithMMI = GetComputerWithInstances(Log.Logger, computer, propertiesArray, className, filterName, auth).Result;
                    // newComputerWithMMI.DateLastSeen = DateTime.Now;  Moved to TestConnection to allow for pre-check
                        newComputers.Add(newComputerWithMMI);
                    }
                    catch (Exception ex)
                    {
                        newComputers.Add(computer);
                        Log.Error($"Error getting MMI data for {computer.Name}.  Error: {ex.Message}");
                    }
                });
            });

            return newComputers;
        }
        private async Task<ComputerModel> GetComputerWithInstances(ILogger logger,
            ComputerModel computer,
            string[] propertiesArray,
            string className,
            string filterName,
            AuthenticationModel auth)
        {
            // No name, no joy.
            if (string.IsNullOrEmpty(computer.Name))
            {
                throw new Exception("Computer name is null or empty.");
            }

            Log.Information($"{computer.DateLastSeen?.AddMinutes(5).ToString("f")} {DateTime.Now.ToString("f")}");
            // Check to see if the computer tested online in the last minute
            if (computer.DateLastSeen is null || computer.DateLastSeen?.AddMinutes(5) < DateTime.Now)
            {
                Log.Information($"Skipping {computer.Name}  because it was tested online in the last 5 minutes.");
                return computer;
            }

            MMIService mmiService = new(logger, computer.Name)
            {
                Authentication = auth,
                PropertiesArray = propertiesArray,
                ClassName = className,
                FilterName = filterName
            };

            // Call the MMIService .
            try
            {
                await mmiService.Execute();
            }
            catch (Exception ex)
            {
                // Log.Error(ex, "Exception from mmiService: {0}", ex.Message);
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
                computer.InstancesDictionary.Add(className, mmiService.Instances);

                // Log the results.
                Log.Verbose($"{computer.Name} returned: {mmiService.Instances.Count}.");
                foreach (CimInstance instance in mmiService.Instances)
                {
                    Log.Verbose("");

                    // If we asked for only some properties, then we can query for only those properties.
                    // Also check that PropertiesArray does not contain "*" which is the wildcard search, asks for everything.
                    if (propertiesArray?.Length > 0 && Array.Exists(propertiesArray, element => element != "*"))
                    {
                        foreach (string property in propertiesArray)
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
