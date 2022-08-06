using DomainTricks_WPF.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DomainTricks_WPF.ViewModels
{
    public class PreferencesViewModel
    {
        private bool _savePassword = true;
        private string _domainName = "Domain Name";
        private string _userName = "User Name";
        private string _password = "Password";
        public string DomainName { get { return _domainName; } set { _domainName = value; } }
        public string UserName { get { return _userName; } set { _userName = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public bool SavePassword
        {
            get { return _savePassword; }
            set { _savePassword = value; }
        }
        public SavePreferencesCommand SavePreferencesCommand { get; set; }
        public CancelPreferencesCommand CancelPreferencesCommand { get; set; }

        // Action to close the window.   Must be set up in the views codebehind.
        public Action CloseAction { get; set; }
        public PreferencesViewModel(ILogger logger)
        {
            Log.Logger = logger;
            this.SavePreferencesCommand = new (logger, this);
            this.CancelPreferencesCommand = new(logger, this);
        }

        public void SavePreferences(string? parameter)
        {
            Log.Information("Saving Preferences");

            // Close the window.
            CloseAction();
            
        }

        public void CancelPreferences(string? parameter)
        {
            Log.Information("Canceling Preferences");

            // Close the window.
            CloseAction();

        }
    }
}
