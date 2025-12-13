namespace SolidZip.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> _availableLanguages;
    [ObservableProperty] private string _selectedLanguage;
    [ObservableProperty] private ObservableCollection<string> _explorerViewStyles;
    [ObservableProperty] private bool _drawViewStyleLoading = true;
    [ObservableProperty] private CultureInfo _currentCulture;
    [ObservableProperty] private string _explorerElementsView;
    [ObservableProperty] private string _currentTheme;
    [ObservableProperty] private FileSizeMeasurement _fileSizeMeasurement;
    [ObservableProperty] private bool _attachPluginsConsole;
    [ObservableProperty] private List<string> _rootDirectoryAdditionalContent;
    [ObservableProperty] private bool _showHiddenDirectories;
    
    private readonly ILuaUiData _uiData;
    private readonly IUserJsonManager _manager;
    private readonly StrongTypedLocalizationManager _localization;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly ILuaEventRaiser _eventRaiser;
    
    private Dictionary<string, string> _localizedExplorerViewStylesDictionary;
    

    public SettingsViewModel(
        ILuaEventRaiser eventRaiser,
        ILogger<SettingsViewModel> logger,
        IOptions<LocalizationOptions> localizationOptions,
        StrongTypedLocalizationManager localization,
        IUserJsonManager manager,
        ILuaUiData uiData) : base(localization)
    {
        _uiData = uiData;
        _logger = logger;
        _localization = localization;
        _manager = manager;
        _eventRaiser = eventRaiser;

        AvailableLanguages = localizationOptions.Value.SupportedCultures.Keys.ToObservable();
        SelectedLanguage = localizationOptions.Value.SupportedCultures
            .First(keyValuePair => Equals(keyValuePair.Value, CultureInfo.CurrentUICulture)).Key;
        
        _eventRaiser.RaiseBackground("settings_view_model_loading");
        LoadSettingsElementsFromTasksAsync()
            .ContinueWith((task) => _eventRaiser.RaiseBackground("settings_view_model_loaded"));
    }

    [RelayCommand]
    private void ChangeTheme()
    {
        
    }

    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        var baseUserData = await _manager.GetAllAsync();
        _logger.LogDebug("Reset settings");
        
        CurrentCulture = baseUserData.CurrentCulture;
        ExplorerElementsView = baseUserData.ExplorerElementsView;
        CurrentTheme = baseUserData.CurrentTheme;
        FileSizeMeasurement = baseUserData.FileSizeMeasurement;
        AttachPluginsConsole = baseUserData.AttachPluginsConsole;
        RootDirectoryAdditionalContent = baseUserData.RootDirectoryAdditionalContent;
        ShowHiddenDirectories = baseUserData.ShowHiddenDirectories;
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
            var userDataToSave = new UserData
            {
                CurrentCulture = CurrentCulture,
                ExplorerElementsView = ExplorerElementsView,
                CurrentTheme = CurrentTheme,
                FileSizeMeasurement = FileSizeMeasurement,
                AttachPluginsConsole = AttachPluginsConsole,
                RootDirectoryAdditionalContent = RootDirectoryAdditionalContent,
                ShowHiddenDirectories = ShowHiddenDirectories
            };
            
            _manager.ChangeAll(userDataToSave);
            _manager.ExpandChanges();
            await _eventRaiser.RaiseAsync("settings_changed", new { UserData = userDataToSave });
            
            _logger.LogInformation("Settings saved successfully");
        
    }

   

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrentCulture):
                _uiData.AddOrUpdate("userdata_current_culture", CurrentCulture?.Name);
                break;
            case nameof(ExplorerElementsView):
                _uiData.AddOrUpdate("settings_explorer_view", ExplorerElementsView);
                break;
            case nameof(CurrentTheme):
                _uiData.AddOrUpdate("settings_current_theme", CurrentTheme);
                break;
            case nameof(FileSizeMeasurement):
                _uiData.AddOrUpdate("settings_file_size_measurement", FileSizeMeasurement.ToString());
                break;
            case nameof(AttachPluginsConsole):
                _uiData.AddOrUpdate("settings_attach_plugins_console", AttachPluginsConsole);
                break;
            case nameof(RootDirectoryAdditionalContent):
                _uiData.AddOrUpdate("settings_root_additional_content", RootDirectoryAdditionalContent);
                break;
            case nameof(ShowHiddenDirectories):
                _uiData.AddOrUpdate("settings_show_hidden_dirs", ShowHiddenDirectories);
                break;
            case nameof(AvailableLanguages):
                _uiData.AddOrUpdate("settings_available_langs", AvailableLanguages.ToArray());
                break;
            case nameof(SelectedLanguage):
                _uiData.AddOrUpdate("settings_selected_lang", SelectedLanguage);
                break;
            case nameof(ExplorerViewStyles):
                _uiData.AddOrUpdate("settings_explorer_view_styles", ExplorerViewStyles);
                break;
        }
        base.OnPropertyChanged(e);
    }
    
    private async Task LoadSettingsElementsFromTasksAsync()
    {
        await LoadUserDataAsync();
        await LoadExplorerViewStylesAsync();
    }

    private async Task LoadUserDataAsync()
    {
        var userData = await _manager.GetAllAsync();
        await Application.Current.Dispatcher.InvokeAsync(() => 
        {
            CurrentCulture = userData.CurrentCulture;
            ExplorerElementsView = userData.ExplorerElementsView;
            CurrentTheme = userData.CurrentTheme;
            FileSizeMeasurement = userData.FileSizeMeasurement;
            AttachPluginsConsole = userData.AttachPluginsConsole;
            RootDirectoryAdditionalContent = userData.RootDirectoryAdditionalContent;
            ShowHiddenDirectories = userData.ShowHiddenDirectories;
        });
    }

    private async Task LoadExplorerViewStylesAsync()
    {
        _localizedExplorerViewStylesDictionary = new()
        {
            { _localization.Grid, nameof(SolidZip.Core.Enums.ExplorerElementsView.Grid) },
            { _localization.Table, nameof(SolidZip.Core.Enums.ExplorerElementsView.Table)  },
            { _localization.List, nameof(SolidZip.Core.Enums.ExplorerElementsView.List) }
        };

        await Application
            .Current
            .Dispatcher
            .InvokeAsync(() =>
            {
                _logger.LogDebug("Start explorer view styles: {values}", _localizedExplorerViewStylesDictionary.Keys.ToArray<object?>());
                return ExplorerViewStyles = _localizedExplorerViewStylesDictionary.Keys.ToObservable();
            });
        
        await Task.Run(async () =>
        {
            var result = await _eventRaiser.RaiseAsync<LuaLoadExplorerElementsView>("load_explorer_elements_view_names");
            await Application.Current
                .Dispatcher.InvokeAsync(() =>
                {
                    foreach (var explorerElementsView in result)
                    {
                        _localizedExplorerViewStylesDictionary.TryAdd(explorerElementsView.localized,
                            explorerElementsView.unlocalized);
                        ExplorerViewStyles.Add(explorerElementsView.localized);
                        _logger.LogDebug("Added explorer view style: {value}", explorerElementsView.unlocalized);
                        OnPropertyChanged(nameof(ExplorerViewStyles)); //Need for updating ui table in lua
                    }
                });
        });
    }
}