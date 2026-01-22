namespace SolidZip.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IDialogHelper _dialogHelper;
    private readonly ILuaEventRaiser _eventRaiser;
    private readonly StrongTypedLocalizationManager _localization;
    private readonly LocalizationOptions _localizationOptions;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IUserJsonManager _manager;
    private readonly IMessenger _messenger;

    private readonly ILuaUiData _uiData;
    private readonly WindowsExplorer _windowsExplorer;
    [ObservableProperty] private bool _attachPluginsConsole;
    [ObservableProperty] private ObservableCollection<string> _availableLanguages;
    [ObservableProperty] private CultureInfo _currentCulture;
    [ObservableProperty] private string _currentTheme;
    [ObservableProperty] private bool _drawViewStyleLoading = true;
    [ObservableProperty] private string _explorerElementsView;
    [ObservableProperty] private ObservableCollection<string> _explorerViewStyles;
    [ObservableProperty] private FileSizeMeasurement _fileSizeMeasurement;

    private Dictionary<string, string> _localizedExplorerViewStylesDictionary;
    [ObservableProperty] private ObservableCollection<string> _rootDirectoryAdditionalContent;
    [ObservableProperty] private string _selectedLanguage;
    [ObservableProperty] private bool _showHiddenDirectories;


    public SettingsViewModel(
        IMessenger messenger,
        WindowsExplorer windowsExplorer,
        ILuaEventRaiser eventRaiser,
        ILogger<SettingsViewModel> logger,
        IOptions<LocalizationOptions> localizationOptions,
        StrongTypedLocalizationManager localization,
        IUserJsonManager manager,
        IDialogHelper dialogHelper,
        ILuaUiData uiData) : base(localization, messenger)
    {
        _messenger = messenger;
        _windowsExplorer = windowsExplorer;
        _uiData = uiData;
        _logger = logger;
        _localization = localization;
        _dialogHelper = dialogHelper;
        _localizationOptions = localizationOptions.Value;
        _manager = manager;
        _eventRaiser = eventRaiser;

        AvailableLanguages = localizationOptions.Value.SupportedCultures.Keys.ToObservable();
        SelectedLanguage = localizationOptions.Value.SupportedCultures
            .First(keyValuePair => Equals(keyValuePair.Value, CultureInfo.CurrentUICulture)).Key;

        _eventRaiser.RaiseBackground("settings_view_model_loading");
        messenger.RegisterAll(this);
        LoadSettingsElementsFromTasksAsync()
            .ContinueWith(task => _eventRaiser.RaiseBackground("settings_view_model_loaded"));
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
        RootDirectoryAdditionalContent = baseUserData.RootDirectoryAdditionalContent.ToObservable();
        ShowHiddenDirectories = baseUserData.ShowHiddenDirectories;
        await _eventRaiser.RaiseAsync("settings_reset_changes", baseUserData);
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
            RootDirectoryAdditionalContent = RootDirectoryAdditionalContent.ToList(),
            ShowHiddenDirectories = ShowHiddenDirectories
        };

        _manager.ChangeAll(userDataToSave);
        _manager.ExpandChanges();
        await _eventRaiser.RaiseAsync("settings_changed", new { UserData = userDataToSave });

        _logger.LogInformation("Settings saved successfully");
        _dialogHelper.Close(ApplicationViews.Settings);
    }

    [RelayCommand]
    private void RemoveRootDirectory(string value)
    {
        RootDirectoryAdditionalContent.Remove(value);
        OnPropertyChanged(nameof(RootDirectoryAdditionalContent));
    }

    [RelayCommand]
    private void AddRootDirectory()
    {
        var result = _windowsExplorer.SelectFolder();

        if (!result.Is(WindowsExplorerDialogResult.Ok))
            return;

        RootDirectoryAdditionalContent.Add(result.Value!);
        OnPropertyChanged(nameof(RootDirectoryAdditionalContent));
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
                ChangeLanguage();
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
            RootDirectoryAdditionalContent = userData.RootDirectoryAdditionalContent.ToObservable();
            ShowHiddenDirectories = userData.ShowHiddenDirectories;
        });
    }

    private async Task LoadExplorerViewStylesAsync()
    {
        _localizedExplorerViewStylesDictionary = new Dictionary<string, string>
        {
            { _localization.Grid, nameof(Core.Enums.ExplorerElementsView.Grid) },
            { _localization.Table, nameof(Core.Enums.ExplorerElementsView.Table) },
            { _localization.List, nameof(Core.Enums.ExplorerElementsView.List) }
        };

        await Application
            .Current
            .Dispatcher
            .InvokeAsync(() =>
            {
                _logger.LogDebug("Start explorer view styles: {values}",
                    _localizedExplorerViewStylesDictionary.Keys.ToArray<object?>());
                return ExplorerViewStyles = _localizedExplorerViewStylesDictionary.Keys.ToObservable();
            });

        await Task.Run(async () =>
        {
            var result =
                await _eventRaiser.RaiseAsync<LuaLoadExplorerElementsView>("load_explorer_elements_view_names");
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

    private void ChangeLanguage()
    {
        var newCulture = _localizationOptions.SupportedCultures[SelectedLanguage];

        if (newCulture.Equals(CultureInfo.CurrentCulture))
            return;

        base.ChangeLanguage(newCulture);
        _logger.LogInformation("Changed language: {value}", newCulture);
    }
}