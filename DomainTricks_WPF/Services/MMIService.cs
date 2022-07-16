using DomainTricks_WPF.Models;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;
using Microsoft.Management.Infrastructure.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services;

public class MMIService
{

    public MMIService(ILogger logger)
    {
        Log.Logger = logger;
    }

    public async Task GetMMI(string computerName)
    {
        if (computerName is null)
        {
            return;
        }

        // Authentication
        string domain = "tuttistudios.com";
        string username = "jennifer";
        string plaintextpassword = "password";
        SecureString securepassword = new();
        foreach (char c in plaintextpassword)
        {
            securepassword.AppendChar(c);
        }
        // create Credentials
        CimCredential Credentials = new(PasswordAuthenticationMechanism.Default, domain, username, securepassword);
        // create SessionOptions using Credentials
        WSManSessionOptions SessionOptions = new();
        SessionOptions.AddDestinationCredentials(Credentials);
        SessionOptions.Timeout = new TimeSpan(0, 0, 10);

        string nameSpace = @"root\cimv2";
        string className = "Win32_ComputerSystem";
        string propertyName = "TotalPhysicalMemory";
        string mmiQuery = "SELECT " + propertyName + " FROM " + className;
        CimSession session = CimSession.Create(computerName, SessionOptions);
        CimInstanceWatcher instanceWatcher = new();
        CimAsyncMultipleResults<CimInstance> multiResult = session.QueryInstancesAsync(nameSpace, "WQL", mmiQuery);
        multiResult.Subscribe(instanceWatcher);

        // Wait for the results
        while (instanceWatcher.IsFinsihed == false && instanceWatcher.IsError == false)
        {
            await Task.Delay(200);
        }
        if (instanceWatcher.IsError)
        {
            Log.Error("Error: {0}", instanceWatcher.ErrorMessage);
        }
        else
        {
            foreach (CimInstance instance in instanceWatcher.Instances)
            {
                if (instance.CimInstanceProperties["TotalPhysicalMemory"].ToString()[0] > ' ')
                {
                    Debug.WriteLine("TotalPhysicalMemory is {0}", instance.CimInstanceProperties["TotalPhysicalMemory"]);
                }
            }
        }
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
        Log.Information("Done");
    }

    public void OnError(Exception e)
    {
        IsError = true;
        ErrorMessage = e.Message;
        Log.Information("Error: " + e.Message);
    }

    public void OnNext(CimInstance value)
    {
        Instances.Add(value);
        Log.Information("Value: " + value);
    }
}


