using Microsoft.Management.Infrastructure;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Models;

public class ComputerModel : ModelBase
{
    private Guid _id;
    private string? _name;
    // When was this computer last reached over MMI.  Null if never.
    private DateTime? _dateLastSeen;

    public Guid Id { get { return _id; } }
    public string? Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public List<string>? OUList { get; set; }

    public DateTime? DateLastSeen
    {
        get { return _dateLastSeen; }
        set
        {
            _dateLastSeen = value;
            OnPropertyChanged(nameof(_dateLastSeen));
        }
    }

    public string? OperatingSystem { get; set; }
    public string? OperatingSystemVersion { get; set; }
    public string? DNSHostName { get; set; }
    
    

    // checks if this ComputerModel points to the computer it is running on
    public static bool IsLocalComputer(string computerName)
    {
        if (computerName is null) return false;
        return computerName.Equals(Environment.MachineName);
    }

    // Key should be the class name used to generate the CimInstance List.
    public Dictionary<string, List<CimInstance>> InstancesDictionary { get; set; } = new();


    public ComputerModel(string computerName, ILogger logger)
    {
        Log.Logger = logger;
        _name = computerName;
        _id = Guid.NewGuid();
        Log.Information($"Creating comptuer with Guid {_id}.");
    }
}