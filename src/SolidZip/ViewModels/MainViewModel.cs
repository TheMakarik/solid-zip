namespace SolidZip.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _currentRealPath = string.Empty;
    [ObservableProperty] private string _currentUiPath = string.Empty;
    [ObservableProperty] private ObservableCollection<FileEntity> _currentExplorerContent = new();
    [ObservableProperty] private bool _canRedo;
    [ObservableProperty] private bool _canUndo;
    [ObservableProperty] private string _searchWatermark = string.Empty;


    private readonly IExplorerStateMachine _explorer;
    private readonly ILuaUiData _uiData;
    private readonly ILuaEventRaiser _raiser;
    private readonly ApplicationViewsLoader _applicationViewsLoader;


    public MainViewModel(IExplorerStateMachine explorer,
        ILuaEventRaiser eventRaiser,
        ILuaUiData uiData,
        ApplicationViewsLoader applicationViewsLoader,
        StrongTypedLocalizationManager localization,
        IOptions<ExplorerOptions> options) : base(localization)
    {
        _explorer = explorer;
        _canRedo = explorer.CanRedo;
        _canUndo = explorer.CanUndo;
        _uiData = uiData;
        _raiser = eventRaiser;
        _applicationViewsLoader = applicationViewsLoader;
        
        var root = default(FileEntity) with {Path = options.Value.RootDirectory, IsDirectory = true};
        GetContentAsync(root);
    }


    [RelayCommand]
    private async Task GetContentAsync(FileEntity directory)
    {

        var result = await _explorer.GetContentAsync(directory);
        await ValidateExplorerResultAsync(result, directory);
    }

    [RelayCommand]
    private async Task GetSearchWatermarkContentAsync()
    {
        await GetContentAsync(default(FileEntity) with { IsDirectory = true, Path = SearchWatermark});
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
            var directoryContent = await _explorer.GetContentAsync(entity, addToHistory: false);
            await ValidateExplorerResultAsync(directoryContent, entity);
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
            var directoryContent = await _explorer.GetContentAsync(entity, addToHistory: false);
            await ValidateExplorerResultAsync(directoryContent, entity);
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
                break;
            case nameof(CurrentRealPath): 
                _uiData.AddOrUpdate("current_real_path", CurrentRealPath );
                if (IsSearching())
                {
                    SearchWatermark = string.Empty;
                    StopSearch();
                }
                  
                break;
            case nameof(SearchWatermark):
                _uiData.AddOrUpdate("search_watermark", SearchWatermark );
                break;
        }
        base.OnPropertyChanged(e);
    }

    private bool IsSearching()
    {
        return SearchWatermark != string.Empty;
    }

    private async Task ValidateExplorerResultAsync(Result<ExplorerResult, IEnumerable<FileEntity>> result, FileEntity directory)
    {
        if (result.Is(ExplorerResult.Success))
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                CurrentExplorerContent = result.Value?.ToObservable() ?? [];
                CurrentRealPath = directory.Path;
                CurrentUiPath = directory.Path;
            });
        }
        
    }
}