namespace SolidZip.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> _availableLanguages;
    [ObservableProperty] private string _selectedLanguage;
    [ObservableProperty] private bool _showLuaConsole;
    [ObservableProperty] private ObservableCollection<string> _explorerViewStyles;
    [ObservableProperty] private bool _drawViewStyleLoading = true;
    
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
        SelectedLanguage = AvailableLanguages.First(language =>
            language == localizationOptions.Value.SupportedCultures
                .First(keyValuePair => Equals(keyValuePair.Value, CultureInfo.CurrentUICulture)).Key);
        
        _eventRaiser.RaiseBackground("settings_view_model_loading");
        LoadSettingsElementsFromTasksAsync()
            .ContinueWith((task) => _eventRaiser.RaiseBackground("settings_view_model_loaded"));
    }

   

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AvailableLanguages):
                _uiData.AddOrUpdate("available_langs", AvailableLanguages.ToArray());
                break;
            case nameof(SelectedLanguage):
                _uiData.AddOrUpdate("selected_lang", SelectedLanguage);
                break;
            case nameof(ShowLuaConsole):
                _uiData.AddOrUpdate("show_lua_console", ShowLuaConsole);
                break;
            case nameof(ExplorerViewStyles):
                _uiData.AddOrUpdate("explorer_view_styles", ExplorerViewStyles);
                break;
        }
        base.OnPropertyChanged(e);
    }
    
    private async Task LoadSettingsElementsFromTasksAsync()
    {
        var showLuaConsole = await _manager.GetAttachPluginsConsoleAsync();
        await Application
            .Current
            .Dispatcher
            .InvokeAsync(() => ShowLuaConsole = showLuaConsole);
        
        await LoadExplorerViewStylesAsync();
        
        var view = await _manager.GetExplorerElementsViewAsync();
        

    }

    private async Task LoadExplorerViewStylesAsync()
    {
        _localizedExplorerViewStylesDictionary = new()
        {
            { _localization.Grid, nameof(ExplorerElementsView.Grid) },
            { _localization.Table, nameof(ExplorerElementsView.Table)  },
            { _localization.List, nameof(ExplorerElementsView.List) }
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