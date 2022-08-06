using DomainTricks_WPF.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Commands
{
    public class SavePreferencesCommand : CommandBase
    {
        public event EventHandler? CanExecuteChanged;

        public PreferencesViewModel ViewModel { get; set; }
        public SavePreferencesCommand(ILogger logger, PreferencesViewModel ViewMOdel)
        {
            Log.Logger = logger;
            this.ViewModel = ViewMOdel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public override void Execute(object? parameter)
        {
            this.ViewModel.SavePreferences(parameter as string);
        }
    }
}
