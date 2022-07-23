using DomainTricks_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DomainTricks_WPF.Commands;

public class MenuClickedCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public MainViewModel ViewModel { get; set; }
    public MenuClickedCommand(MainViewModel ViewMOdel)
    {
        this.ViewModel = ViewMOdel;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }
    
    public void Execute(object? parameter)
    {
        this.ViewModel.MenuClickedCommandAction();
    }
}
