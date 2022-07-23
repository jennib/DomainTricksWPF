using DomainTricks_WPF.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DomainTricks_WPF.Commands;

public class MenuClickedCommand : CommandBase
{
    public event EventHandler? CanExecuteChanged;

    public MainViewModel ViewModel { get; set; }
    public MenuClickedCommand(ILogger logger, MainViewModel ViewMOdel)
    {
        this.ViewModel = ViewMOdel;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }
    
    public override void Execute(object? parameter)
    {
        if (parameter is string)
        {
            this.ViewModel.MenuClickedCommandAction(parameter as string);
        }
    }

}
