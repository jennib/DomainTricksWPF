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
        private string _freeSpaceCriticalPercent;
        private string _freeSpaceWarningPercent;

        public string FreeSpaceCriticalPercent
        {
            get { return _freeSpaceCriticalPercent; }
            set
            {
                _freeSpaceCriticalPercent = value ;
                OnPropertyChanged(nameof(FreeSpaceCriticalPercent));
            }
        }
        public string FreeSpaceWarningPercent
        {
            get { return _freeSpaceWarningPercent; }
            set
            {
                _freeSpaceWarningPercent = value;
                OnPropertyChanged(nameof(FreeSpaceWarningPercent));
            }
        }

        // Action to close the window.   Must be set up in the views codebehind.
        public Action? CloseAction { get; set; }

        public RelayCommand SavePreferencesCommand { get; set; }

        // Should the Save button be enabled.
        public bool CanSave(object value)
        {
       // Check that strings that should contain numbers, do.
            if (int.TryParse(FreeSpaceCriticalPercent, out _) && int.TryParse(FreeSpaceWarningPercent, out _))
            {
                return true;
            }
            return false;
        }
        
        public PreferencesViewModel(ILogger logger)
        {
            Log.Logger = logger;
            // Load from settings on disk.
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSave);

            // TODO: Get values from AuthenticationModel
          
            FreeSpaceCriticalPercent = Properties.Settings.Default.DiskFreePercentCritical.ToString();
            FreeSpaceWarningPercent = Properties.Settings.Default.DiskFreePercentWarning.ToString();
        }

        public void SavePreferences(object value)
        {
            // Save values to settings on disk.
            int FreeSpaceCritical;
            int FreeSpaceWarning;
            if ( int.TryParse(FreeSpaceCriticalPercent, out FreeSpaceCritical) {
                Properties.Settings.Default.DiskFreePercentCritical = FreeSpaceCritical;
            }
            if ( int.TryParse(FreeSpaceWarningPercent, out FreeSpaceWarning){
                Properties.Settings.Default.DiskFreePercentWarning = FreeSpaceWarning;
            }
            Properties.Settings.Default.Save();

            // Close the window.
            CloseAction?.Invoke();
        }
    }
}
