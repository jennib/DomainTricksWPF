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
        private bool _runAsLocalUser = true;
        private string _domainName = "Domain Name";
        private string _userName = "User Name";
        private string _password = "Password";
        private bool _shouldRememberPassword = true;

        public bool RunAsLocalUser
        {
            get { return _runAsLocalUser; }
            set
            {
                _runAsLocalUser = value;
                OnPropertyChanged(nameof(RunAsLocalUser));
            }
        }
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
        public bool ShouldRememberPassword
        {
            get { return _shouldRememberPassword; }
            set
            {
                _shouldRememberPassword = value;
                OnPropertyChanged(nameof(ShouldRememberPassword));
            }
        }

        // Action to close the window.   Must be set up in the views codebehind.
        public Action? CloseAction { get; set; }

        public RelayCommand SavePreferencesCommand { get; set; }

        // Should the Save button be enabled.
        public bool CanSave(object value)
        {
            if (RunAsLocalUser)
            {
                return true;
            }
            if (string.IsNullOrEmpty(DomainName) || string.IsNullOrEmpty(UserName)
                || string.IsNullOrEmpty(Password))
            {
                return false;
            }
            return true;
        }

        public PreferencesViewModel(ILogger logger)
        {
            Log.Logger = logger;
            // Load from settings on disk.
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSave);

            // TODO: Get values from AuthenticationModel
            RunAsLocalUser = Properties.Settings.Default.RunAsLocalUser;
            DomainName = Properties.Settings.Default.DomainName;
            UserName = Properties.Settings.Default.UserName;
            Password = Properties.Settings.Default.Password;
            ShouldRememberPassword = Properties.Settings.Default.ShouldRememberPassword;
        }

        public void SavePreferences(object value)
        {
            Log.Information("Saving Preferences");
            if (RunAsLocalUser == false)
            {
                if (string.IsNullOrWhiteSpace(DomainName) || string.IsNullOrWhiteSpace(UserName)
                    || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please fill in all fields");
                    return;
                }
            }

            // Save values to settings on disk.
            Properties.Settings.Default.RunAsLocalUser = RunAsLocalUser;
            if (RunAsLocalUser)
            {
                Properties.Settings.Default.DomainName = "";
                Properties.Settings.Default.UserName = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.ShouldRememberPassword = false;
            }
            else
            {
                Properties.Settings.Default.DomainName = DomainName;
                Properties.Settings.Default.UserName = UserName;
                if (ShouldRememberPassword)
                {
                    Properties.Settings.Default.Password = Password;
                }
                Properties.Settings.Default.ShouldRememberPassword = ShouldRememberPassword;
            }
            Properties.Settings.Default.Save();

            // Close the window.
            CloseAction?.Invoke();
        }
    }
}
