using DomainTricks_WPF.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DomainTricks_WPF.Services;

namespace DomainTricks_WPF.ViewModels
{
    public class PreferencesViewModel : ViewModelBase
    {
        private string _freeSpaceCriticalPercent;
        private string _freeSpaceWarningPercent;
        private string _timerMinutes;
        private MainViewModel _mainViewModel;

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
        
        public  string TimerMinutes
        {
            get { return _timerMinutes; }
            set
            {
                _timerMinutes = value;
                OnPropertyChanged(nameof(TimerMinutes));
            }
        }

        // Action to close the window.   Must be set up in the views codebehind.
        public Action? CloseAction { get; set; }

        public RelayCommand SavePreferencesCommand { get; set; }

        // Should the Save button be enabled.
        public bool CanSave(object value)
        {
       // Check that strings that should contain numbers, do.
            if (int.TryParse(FreeSpaceCriticalPercent, out _) && int.TryParse(FreeSpaceWarningPercent, out _) && int.TryParse(TimerMinutes, out _))
            {
                return true;
            }
            return false;
        }
        
        public PreferencesViewModel(ILogger logger, MainViewModel mainViewModel)
        {
            Log.Logger = logger;
            _mainViewModel = mainViewModel;
            // Load from settings on disk.
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSave);
            TimerMinutes = Properties.Settings.Default.TimerMinutes.ToString();
            FreeSpaceCriticalPercent = Properties.Settings.Default.DiskFreePercentCritical.ToString();
            FreeSpaceWarningPercent = Properties.Settings.Default.DiskFreePercentWarning.ToString();
            
        }

        public void SavePreferences(object value)
        {
            // Save values to settings on disk.
            int freeSpaceCritical;
            int freeSpaceWarning;
            int timerMinutes;
            
            if ( int.TryParse(FreeSpaceCriticalPercent, out freeSpaceCritical)) {
                Properties.Settings.Default.DiskFreePercentCritical = freeSpaceCritical;
            }
            if ( int.TryParse(FreeSpaceWarningPercent, out freeSpaceWarning)) {
                Properties.Settings.Default.DiskFreePercentWarning = freeSpaceWarning;
            } if ( int.TryParse(TimerMinutes, out timerMinutes)) {
                Properties.Settings.Default.TimerMinutes = timerMinutes;
            }
            Properties.Settings.Default.Save();


            // Change the timerSpan in BackgrounService
            _mainViewModel.TimerInterval = timerMinutes.ToString();

            // Close the window.
            CloseAction?.Invoke();
        }
    }
}
