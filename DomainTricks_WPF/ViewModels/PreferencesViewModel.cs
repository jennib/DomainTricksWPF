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
    public class PreferencesViewModel : ViewModelBase
    {
        private bool _savePassword = true;
        private string _domainName = "Domain Name";
        private string _userName = "User Name";
        private string _password = "Password";
        public string DomainName
        {
            get { return _domainName; }
            set
            {
                _domainName = value;
                OnPropertyChanged(nameof(DomainName));
            }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public bool ShouldSavePassword
        {
            get { return _savePassword; }
            set { _savePassword = value;
                OnPropertyChanged(nameof(ShouldSavePassword));
            }
        }
        public RelayCommand SavePreferencesCommand { get; set; }
        public bool CanSave(object value) { 
                if (string.IsNullOrEmpty(DomainName) || string.IsNullOrEmpty(UserName) 
                    || string.IsNullOrEmpty(Password))
                { 
                    return false; 
                }
                return true;
            } 
        //public SavePreferencesCommand SavePreferencesCommand { get; set; }
        //public CancelPreferencesCommand CancelPreferencesCommand { get; set; }

        // Action to close the window.   Must be set up in the views codebehind.
        public Action CloseAction { get; set; }
        public PreferencesViewModel(ILogger logger)
        {
            Log.Logger = logger;
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSave);
            DomainName = Properties.Settings.Default.DomainName;
            UserName = Properties.Settings.Default.UserName;

            Password = Properties.Settings.Default.Password;
        }

        public void SavePreferences(object value)
        {
            Log.Information("Saving Preferences");
            if (string.IsNullOrEmpty(DomainName) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }
            Properties.Settings.Default.DomainName = DomainName;
            Properties.Settings.Default.UserName = UserName;
            Properties.Settings.Default.Password = Password;
            Properties.Settings.Default.Save();
            // Close the window.
            CloseAction();

        }

        public void CancelPreferences(object value)
        {
            Log.Information("Canceling Preferences");

            // Close the window.
            CloseAction();

        }
    }
}
