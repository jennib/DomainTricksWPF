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

    public Guid Id { get => _id; }
    public string? Name
    {
        get { return _name; }
        set { _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    
   // private List<CimInstance>? Instances { get; set; }
    
    public Dictionary<string,List<CimInstance>> InstancesDictionary { get; set; } = new();


    public ComputerModel(ILogger logger, Guid id)
    {
        _id = id;
        Log.Information($"Creating comptuer with Guid {_id}.");
    }
}