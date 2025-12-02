using Material.Icons;

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
    
    
    public MainViewModel(IExplorerStateMachine explorer, 
        ILuaEventRaiser eventRaiser, 
        ILuaUiData uiData,
        StrongTypedLocalizationManager localization,
        IOptions<ExplorerOptions> options) : base(localization)
    {
        _explorer = explorer;
        _canRedo = explorer.CanRedo;
        _canUndo = explorer.CanUndo;
        _uiData = uiData;
        _raiser = eventRaiser;

        var root = options.Value.RootDirectory.ToDirectoryFileEntity(rootDirectory: true);
        _explorer.GetContentAsync(root)
            .AsTask().ContinueWith(async (task) =>
            {
                await ValidateExplorerResultAsync(task.Result, root);
            });
    }


    [RelayCommand]
    private async Task GetContentAsync(FileEntity directory)
    {
        await Task.Run(async () =>
        {
            var result = await _explorer.GetContentAsync(directory);
            await ValidateExplorerResultAsync(result, directory);
        });
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
                    SearchWatermark = string.Empty;
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
                CurrentExplorerContent = result.Value?.ToObservable() ?? []);
            CurrentRealPath = directory.Path;
        }
    }
}