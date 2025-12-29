
namespace SolidZip.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    private const int ExplorerElementsHeightMax = 30;
    private const int ExplorerElementsHeightMin = 15;
    
    [ObservableProperty] private string _currentRealPath = string.Empty;
    [ObservableProperty] private string _currentUiPath = string.Empty;
    [ObservableProperty] private ObservableCollection<FileEntity> _currentExplorerContent = new();
    [ObservableProperty] private bool _canRedo;
    [ObservableProperty] private bool _canUndo;
    [ObservableProperty] private string _searchWatermark = string.Empty;
    [ObservableProperty] private FileEntity _selectedEntity;
    [ValueRange(ExplorerElementsHeightMin, ExplorerElementsHeightMax)][ObservableProperty] private int _explorerElementsHeight = 0;

    private readonly IExplorerStateMachine _explorer;
    private readonly ILuaUiData _uiData;
    private readonly ILuaEventRaiser _raiser;
    private readonly ApplicationViewsLoader _applicationViewsLoader;
    private readonly IUserJsonManager _userJsonManager;
    private readonly ILogger<MainViewModel> _logger;


    public MainViewModel(
        ILogger<MainViewModel> logger,
        IMessenger messenger,
        IUserJsonManager userJsonManager,
        IExplorerStateMachine explorer,
        ILuaEventRaiser eventRaiser,
        ILuaUiData uiData,
        ApplicationViewsLoader applicationViewsLoader,
        StrongTypedLocalizationManager localization,
        IOptions<ExplorerOptions> options) : base(localization, messenger)
    {
        _explorer = explorer;
        _canRedo = explorer.CanRedo;
        _logger = logger;
        _canUndo = explorer.CanUndo;
        _uiData = uiData;
        _raiser = eventRaiser;
        _userJsonManager = userJsonManager;
        _applicationViewsLoader = applicationViewsLoader;
        var root = default(FileEntity) with {Path = options.Value.RootDirectory, IsDirectory = true};
        GetContentAsync(root);
    }
    
    [RelayCommand]
    private async Task GetContentAsync(FileEntity directory)
    {
        if (!IsElementsHeightLoaded())
            await LoadElementsHeightAsync();
        
        var result = await _explorer.GetContentAsync(directory);
        await ValidateExplorerResultAsync(result, directory);
    }

    [RelayCommand]
    private async Task GetSearchWatermarkContentAsync()
    {
        await GetContentAsync(default(FileEntity) with { IsDirectory = true, Path = SearchWatermark});
    }


    [RelayCommand]
    private void ChangeExplorerElementsHeight(int value)
    {
        if (ExplorerElementsHeight >= ExplorerElementsHeightMax && value > 0)
            return;

        if (ExplorerElementsHeight <= ExplorerElementsHeightMin && value < 0)
            return;
        
        ExplorerElementsHeight += value;
        _userJsonManager.ChangeExplorerElementsHeight(ExplorerElementsHeight);
        _logger.LogDebug("Changing ExplorerElementsHeight to {val}", ExplorerElementsHeight);
    }
    
    [RelayCommand]
    private void OpenSettings()
    {
        _applicationViewsLoader
            .Load<Window>(ApplicationViews.Settings)
            .ShowDialog();
    }

    [RelayCommand]
    private async Task RedoAsync()
    {
        await Task.Run(async () =>
        {
            if (!CanRedo)
                return;
            
            var entity = _explorer.Redo();
            var directoryContent = await _explorer.GetContentAsync(entity);
            await ValidateExplorerResultAsync(directoryContent, entity, addToHistory: false);
            _raiser.RaiseBackground("redo_executed", new {entity = entity, content = directoryContent.Value});
        });
    }
    
    [RelayCommand]
    private async Task UndoAsync()
    {
        await Task.Run(async () =>
        {
            if (!_explorer.CanUndo)
                return;
            
            var entity = _explorer.Undo();
            var directoryContent = await _explorer.GetContentAsync(entity);
            await ValidateExplorerResultAsync(directoryContent, entity, addToHistory: false);
            _raiser.RaiseBackground("undo_executed", new {entity = entity, content = directoryContent.Value});
        });
    }

    [RelayCommand]
    private async Task StartSearchAsync()
    {
        _explorer.BeginSearch();
        await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await Task.Run(async () =>
        {
            var result = _explorer.Search(CurrentRealPath, CurrentUiPath.Substring(CurrentUiPath.Length));
            await Application.Current.Dispatcher.InvokeAsync(() => SearchWatermark = result.Path);
        });
    }
    
    [RelayCommand]
    private void StopSearch()
    {
        _explorer.EndSearch();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrentUiPath):
                _uiData.AddOrUpdate("current_ui_path", CurrentUiPath);
                _raiser.RaiseBackground("current_ui_path_changed", new { current_ui_path = CurrentUiPath});
                break;
            case nameof(CurrentRealPath): 
                _uiData.AddOrUpdate("current_real_path", CurrentRealPath );
                _raiser.RaiseBackground("current_real_path_changed", new { current_real_path = CurrentRealPath});
                if (IsSearching())
                {
                    SearchWatermark = string.Empty;
                    StopSearch();
                }
                  
                break;
            case nameof(SearchWatermark):
                _uiData.AddOrUpdate("search_watermark", SearchWatermark );
                _raiser.RaiseBackground("search_watermark_changed", new { search_watermark = SearchCommand});
                break;
            case nameof(ExplorerElementsHeight):
                _uiData.AddOrUpdate("explorer_height", ExplorerElementsHeight);
                _raiser.RaiseBackground("explorer_height_changed", new { height = ExplorerElementsHeight});
                break;
            case nameof(SelectedEntity):
                _uiData.AddOrUpdate("selected_entity_path", SelectedEntity.Path);
                _raiser.RaiseBackground("selected_entity_path_changed", new { path = SelectedEntity.Path});
                break;
        }
        base.OnPropertyChanged(e);
    }

    private bool IsSearching()
    {
        return SearchWatermark != string.Empty;
    }

    private async Task ValidateExplorerResultAsync(Result<ExplorerResult, IEnumerable<FileEntity>> result, FileEntity directory, bool addToHistory = true)
    {
        if (result.Is(ExplorerResult.Success))
        { 
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CurrentExplorerContent = result.Value?.ToObservable() ?? [];
                CurrentRealPath = directory.Path = Environment.ExpandEnvironmentVariables(directory.Path);
                CurrentUiPath = CurrentRealPath;
                CanUndo = _explorer.CanUndo;
                CanRedo = _explorer.CanRedo;
            });
            if(addToHistory)
                _explorer.AddToHistory(directory);
        }
        
    }
    
    private async Task LoadElementsHeightAsync()
    {
        ExplorerElementsHeight = await _userJsonManager.GetExplorerElementsHeightAsync();
    }

    private bool IsElementsHeightLoaded() => ExplorerElementsHeight != 0;
}