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
    private async Task GetContentAsync(object entity)
    {
        //I don't know why but after adding ItemDoubleClickBehavior randomly
        //ExplorerView returns string instead of FileEntity
        var directory = entity is string path
            ? path.ToDirectoryFileEntity()
            : (FileEntity)entity;
        
        await Task.Run(async () =>
        {
            var result = await _explorer.GetContentAsync(directory);

            if (result.Is(ExplorerResult.Success))
                await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                    CurrentExplorerContent = new ObservableCollection<FileEntity>(result.Value!));
        });
    }
    
}