﻿using DomainTricks_WPF.Commands;
using DomainTricks_WPF.Helpers;
using DomainTricks_WPF.Models;
using DomainTricks_WPF.Services;
using Microsoft.Management.Infrastructure;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DomainTricks_WPF.ViewModels;

public class MainViewModel : ViewModelBase
{
    private BackgroundService? _backgroundService;
    private string? _title = "Domain Tricks";
    private string? _statusBarText = string.Empty;
    private Visibility _progressBarShouldBeVisible = Visibility.Hidden;
    private int _progressBarMaximum = 100;
    private int _progressBarPercent = 0;
    private string? _filterString = String.Empty;
    private ICollectionView? _computerCollectionView = null;
    private TimeSpan _timerInterval;

    // The main list of Computers
    private List<ComputerModel> _computers = new();

    public List<ComputerModel> Computers
    {
        get { return _computers; }
        set
        {
            _computers = value;
            OnPropertyChanged(nameof(Computers));
        }
    }

    public string? Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }
    public bool IsPaused { get; set; }

    public string? StatusBarText
    {
        get { return _statusBarText; }
        set
        {
            _statusBarText = value;
            OnPropertyChanged(nameof(StatusBarText));
        }
    }

    public Visibility ProgressBarShouldBeVisible
    {
        get { return _progressBarShouldBeVisible; }
        set
        {
            _progressBarShouldBeVisible = value;
            OnPropertyChanged(nameof(ProgressBarShouldBeVisible));
        }
    }

    public int ProgressBarMaximum
    {
        get { return _progressBarMaximum; }
        set
        {
            _progressBarMaximum = value;
            OnPropertyChanged(nameof(ProgressBarMaximum));
        }
    }

    public int ProgressBarPercent
    {
        get { return _progressBarPercent; }
        set
        {
            _progressBarPercent = value;
            OnPropertyChanged(nameof(ProgressBarPercent));
        }
    }

    public string? FilterString
    {
        get { return _filterString; }
        set
        {
            _filterString = value;
            if (ComputerCollectionView is not null)
            {
                ComputerCollectionView.Refresh();
            }
            OnPropertyChanged(nameof(FilterString));
        }
    }
    
    public  string? TimerInterval {
        get { return _timerInterval.Minutes.ToString(); }
        set
        {
            if (int.TryParse(value, out var interval))
            {
                _timerInterval = TimeSpan.FromMinutes(interval);
                 _backgroundService.StopAsync();
                _backgroundService?.Start(TimeSpan.FromMinutes(interval));
            }
            OnPropertyChanged(nameof(TimerInterval));
        }
    }

    // For filtering and sorting of computers.
    public ICollectionView? ComputerCollectionView
    {
        get { return _computerCollectionView; }
        private set
        {
            _computerCollectionView = value;
            OnPropertyChanged(nameof(ComputerCollectionView));
        }

    }

    public MenuClickedCommand MenuClickedCommand { get; set; }

    public MainViewModel(ILogger logger)
    {
        Log.Logger = logger;
        Log.Information("MainViewModel start.");

        this.MenuClickedCommand = new MenuClickedCommand(logger, this);

        // Read the refresh timer value in minutes from settings.
        int timerMinutes = Properties.Settings.Default.TimerMinutes;
        if (timerMinutes <= 0) timerMinutes = 10;
        
        RefreshComputers();

        _backgroundService = new BackgroundService(logger, this);

        TimerInterval = timerMinutes.ToString();

        //ComputerModel StartComputer = new("Loading", logger);
        //List<ComputerModel> StartComputerList = new() {
        //    StartComputer
        //};
        //Computers = StartComputerList;

        //SetupCollectionView();

        // Test Timer
        //TestTimer(logger);

        // Test the ComputersService
        // Log.Information("Testing the ComputersService.");
        //TestComputersService(logger);

        //// Test the Computer Model.
        //Log.Information("Test the ComputerModel.");
        //ComputerModel computer = new("MyCompuyter", logger);

        //// Test the Domain Service
        //Log.Information("Test the DomainService.");
        //TestDomainService(logger);

        //// Test the Directory Search
        //Log.Information("Test the Directory Search.");
        //TestADSearcher(logger);

        //// Test the MMIService.
        //Log.Information("Test the MMIService.");
        //TestMMI(logger, computer);
    }

    public async Task RefreshComputers()
    {
        Log.Information("RefreshComputers start.");
        Helper.SetMouseCursorToWait();

        ProgressBarShouldBeVisible = Visibility.Visible;
        ProgressBarPercent = 50;
        ProgressBarMaximum = 100;
        StatusBarText = "Refreshing Computers...";
        
        DomainService domainService = new(Log.Logger);
        string domainPath = await DomainService.GetCurrentDomainPathAsync();
        StatusBarText = $"Refreshing Computers in {domainPath}...";
        ComputersService computers = new(Log.Logger);
        List<ComputerModel> computersList = await computers.GetComputers(domainPath);

        this.Computers.Clear();
        this.Computers.AddRange(computersList);
        //Computers = computersList;
        SetupCollectionView();
        Helper.SetMouseCursorToNormal();
        StatusBarText = $"Updated {String.Format("{0:f}", DateTime.Now)}";
        ProgressBarShouldBeVisible = Visibility.Hidden;
        ProgressBarPercent = 100;
        ProgressBarMaximum = 100;
        OnPropertyChanged(nameof(Computers));
    }

    // Test the ComputersService.
    public async void TestComputersService(ILogger logger)
    {
        DomainService domainService = new(logger);
        string domainPath = await DomainService.GetCurrentDomainPathAsync();

        ComputersService computers = new(logger);
        List<ComputerModel> computersList = await computers.GetComputers(domainPath);
        //foreach (ComputerModel computer in computersList)
        //{
        //    Log.Information($"Computer: {computer.Name}: {computer.InstancesDictionary.Count} instances.  Last seen {computer.DateLastSeen?.ToString("f")}");
        //}
        this.Computers.Clear();
        this.Computers.AddRange(computersList);
        Computers = computersList;
        SetupCollectionView();
        OnPropertyChanged(nameof(Computers));
    }

    // Test the Timer in BackgroundTask
    public async void TestTimer(ILogger logger)
    {
        Log.Information("Test the Timer in BackgroundTask.");
        BackgroundService task = new BackgroundService(logger, this);

        task.Start(TimeSpan.FromMinutes(1));

        await Task.Delay(TimeSpan.FromSeconds(10));
        await task.StopAsync();
    }

    // Test the Domain Service call
    async void TestDomainService(ILogger logger)
    {
        DomainService domainService = new(logger);
        string domainPath = await DomainService.GetCurrentDomainPathAsync();
        string domainName = DomainService.DomainNameFromLDAPPath(domainPath);
        Log.Information($"Domain path: {domainPath} name: {domainName}");

    }

    // Test the Active Directory Searcher;
    async void TestADSearcher(ILogger logger)
    {

        ADService adService = new(logger);
        string domainPath = await DomainService.GetCurrentDomainPathAsync();
        List<ComputerModel> computerModels = await adService.GetListOfComputersFromADAsync(domainPath);
        Log.Information($"computerModels has {computerModels.Count()} computers.");

    }

    // Test the Microsoft Management Infrastructure call.
    async void TestMMI(ILogger logger, ComputerModel computer)
    {
        // Prepare to call MMIService.
        string computerName = "RELIC-PC";
        string[] PropertiesArray = { "*" };//{"TotalPhysicalMemory"};
        string ClassName = "Win32_Volume"; //"Win32_ComputerSystem";
        string FilterName = "";

        MMIService mmiService = new(logger, computerName)
        {
            PropertiesArray = PropertiesArray,
            ClassName = ClassName,
            FilterName = FilterName
        };

        // Call the MMIService .
        try
        {
            await mmiService.Execute();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception in TestMMI: {0}", ex.Message);
        }

        // Check the Results.
        // The Instances property is of type CimInstance.  
        // It can have multiple Instances and each instance can have multiple Properties.
        if (mmiService.IsError == true)
        {
            Log.Error($"{computerName} returned error: {mmiService.ErrorMessage}");
        }
        else
        {
            // Add to the ComputerMOdel.
            computer.InstancesDictionary.Add(ClassName, mmiService.Instances);

            Log.Verbose($"{computerName} returned: {mmiService.Instances.Count}.");
            foreach (CimInstance instance in mmiService.Instances)
            {
                Log.Verbose("");

                // If we asked for only some properties, then we can query for only those properties.
                // Also check that PropertiesArray does not contain "*" which is the wildcard search, asks for everything.
                if (PropertiesArray?.Length > 0 && Array.Exists(PropertiesArray, element => element != "*"))
                {
                    foreach (string property in PropertiesArray)
                    {
                        Log.Verbose($"{property} = {instance.CimInstanceProperties[property].Value}");
                    }
                }
                else
                {
                    // Show us all the properties for the instance.
                    foreach (CimProperty property in instance.CimInstanceProperties)
                    {
                        Log.Verbose($"Name: {property.Name}:{property.Name?.GetType().ToString()} value: {property.Value}:{property.Value?.GetType().ToString()} ");
                    }
                }
            }
        }
    }

    public async void MenuClickedCommandAction(string? parameter)
    {
        Log.Information($"MenuClickedCommandAction {parameter}");
        switch (parameter)
        {
            case "About":
                Log.Information("About");
                break;
            case "Exit":
                Log.Information("Exit");
                break;
            case "Preferences":
                Log.Information("Preferences");
                OpenPreferences(null);
                break;
            case "Help":
                Log.Information("Help");
                break;
            case "RunJob":
                Log.Information("RunJob");
                await RefreshComputers();
                break;
            case "PauseProcessing":
                Log.Information("PauseProcessing");
                break;
            default:
                Log.Information("Unknown");
                break;
        }
    }

      void OpenPreferences(object parameter)
    {
        Log.Information($"Open Preferences {parameter}");

        PreferencesViewModel preferencesViewModel = new(Log.Logger,this);
        PreferencesView preferencesView = new(preferencesViewModel);
        preferencesView.Owner = Application.Current.MainWindow;
        preferencesView.ShowDialog();
    }

    private void SetupCollectionView()
    {
        // setup filtering and sorting
        ComputerCollectionView = CollectionViewSource.GetDefaultView(Computers);
        ComputerCollectionView.Filter = _filterComputers;
        ComputerCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
    }

    private bool _filterComputers(object obj)
    {
        if (obj is ComputerModel computerViewModel)
        {
            return computerViewModel.Name.Contains(_filterString, StringComparison.InvariantCultureIgnoreCase);
        }
        return false;
    }
}


