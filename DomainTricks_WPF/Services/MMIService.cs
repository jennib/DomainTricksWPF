﻿using DomainTricks_WPF.Models;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services;

public class MMIService
{
    // Return Properties.
    public bool IsError = false;

    public string? ErrorMessage = string.Empty;

    public List<CimInstance> Instances = new();

    // Set Properties.
    public string? ClassName { get; set; }

    public string[]? PropertiesArray { get; set; }
    public string? FilterName { get; set; }

    // Set in the constructor.
    public string ComputerName { get; }


    public MMIService(ILogger logger, string computerName)
    {
        ComputerName = computerName;
        Log.Logger = logger;
    }

    public async Task<bool> TestConnection()
    {
        CimSessionOptions SessionOptions = new() { Timeout = TimeSpan.FromSeconds(1) };
        
        CimSession session = CimSession.Create(ComputerName, SessionOptions);
        bool result = false;
        try
        {
            result = session.TestConnection();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error testing connection to {ComputerName}", ComputerName);
            return false;
        }

        return result;
    }
    public async Task Execute()
    {
        // Generate the properties string if Properties is not null.
        string? propertiesString;
        if (PropertiesArray is not null && PropertiesArray?.Length > 0)
        {
            propertiesString = string.Join(",", PropertiesArray);
        }
        else
        {
            propertiesString = "*";
        }

        // Make sure we have what we need.
        if (string.IsNullOrEmpty(ComputerName) || string.IsNullOrEmpty(ClassName) || string.IsNullOrEmpty(propertiesString))
        {
            IsError = true;
            ErrorMessage = "ComputerName, Class, and Properties are required";
            Log.Error(ErrorMessage);
            throw new Exception("ComputerName, Class, and Properties are required"); ;
        }

        WSManSessionOptions SessionOptions = new();

        // MMI Search Timeout.
        SessionOptions.Timeout = new TimeSpan(0, 0, 1);

        string nameSpace = @"root\cimv2";

        string mmiQuery = "SELECT " + propertiesString + " FROM " + ClassName;

        // Append the filter if one exists.
        if (string.IsNullOrEmpty(FilterName) == false)
        {
            mmiQuery += $" WHERE {FilterName}";
        }
        Log.Information($"{ComputerName} MMI Query: {mmiQuery}");

        CimSession session = CimSession.Create(ComputerName, SessionOptions);
        CimInstanceWatcher instanceWatcher = new();
        CimAsyncMultipleResults<CimInstance> multiResult = session.QueryInstancesAsync(nameSpace, "WQL", mmiQuery);
        multiResult.Subscribe(instanceWatcher);

        // Wait for the results.
        while (instanceWatcher.IsFinsihed == false && instanceWatcher.IsError == false)
        {
            await Task.Delay(200);
        }

        IsError = instanceWatcher.IsError;
        ErrorMessage = instanceWatcher.ErrorMessage;
        Instances = instanceWatcher.Instances;

        if (instanceWatcher.IsError)
        {
            IsError = true;
            ErrorMessage = instanceWatcher.ErrorMessage;
            Log.Error("Error: {0}", instanceWatcher.ErrorMessage);
            throw new Exception(instanceWatcher.ErrorMessage);
        }
        else
        {
            Log.Information($"There are {instanceWatcher.Instances.Count()} instances.");
            foreach (CimInstance instance in instanceWatcher.Instances)
            {
                Log.Information($" - There are {instance.CimInstanceProperties.Count()} properties.");
            }
        }
        return;
    }

}


/// <summary>
/// Observes a MMI instance to allow for aync 
/// </summary>
class CimInstanceWatcher : IObserver<CimInstance>
{
    public bool IsFinsihed { get; set; }
    public bool IsError { get; set; }

    public string? ErrorMessage = string.Empty;

    public List<CimInstance> Instances;

    public CimInstanceWatcher()
    {
        IsFinsihed = false;
        IsError = false;
        Instances = new List<CimInstance>();
    }

    public void OnCompleted()
    {
        IsFinsihed = true;
        Log.Verbose("CimInstanceWatcher is Done");
    }

    public void OnError(Exception e)
    {
        IsError = true;
        ErrorMessage = e.Message;
        Log.Error("Error: " + e.Message);
    }

    public void OnNext(CimInstance value)
    {
        Instances.Add(value);
        //Log.Information("Value: " + value);
    }
}



