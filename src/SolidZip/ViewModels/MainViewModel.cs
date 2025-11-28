namespace SolidZip.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _currentRealPath = string.Empty;
    [ObservableProperty] private string _currentUiPath = string.Empty;
    [ObservableProperty] private ObservableCollection<FileEntity> _currentExplorerContent = new();
    [ObservableProperty] private bool _canRedo;
    [ObservableProperty] private bool _canUndo;
    
    
    private readonly IExplorerStateMachine _explorer;
    
    public MainViewModel(IExplorerStateMachine explorer, ILuaEventRaiser eventRaiser, IOptions<ExplorerOptions> options)
    {
        _explorer = explorer;
        _canRedo = explorer.CanRedo;
        _canUndo = explorer.CanUndo;

        _explorer.GetContentAsync(options.Value.RootDirectory.ToDirectoryFileEntity(rootDirectory: true));
    }


    [RelayCommand]
    private async Task GetContentAsync(FileEntity directory)
    {
        await Task.Run(async () =>
        {
            var result = await _explorer.GetContentAsync(directory);

            if (result.Is(ExplorerResult.Success))
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                    CurrentExplorerContent = result.Value?.ToObservable() ?? []);
                CurrentRealPath = directory.Path;
            }
              
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
            await AddUndoOrRedoContentAsync(entity);
        });
    }

    private async Task AddUndoOrRedoContentAsync(FileEntity entity)
    {
        throw new NotImplementedException();
    }
}