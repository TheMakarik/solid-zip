namespace SolidZip.ViewModels;

public sealed partial class MainWindowViewModel 
    : ViewModelBase,
        IRecipient<ShowUnauthorizedAccessMessage>,
        IRecipient<UpdateCurrentDirectoryMessage>,
        IRecipient<GetCurrentDirectoryPathMessage>
{
    #region Fleids
    
    private IServiceScope _directorySearcherScope;

    #endregion
    
    #region Services

    private readonly IMessenger _messenger;
    private readonly ViewModelLocator _locator;
    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region Properties for view

    [ObservableProperty] private string? _currentDirectoryBeforeSearching = null;
    [ObservableProperty] private UserControl _explorerControl;
    [ObservableProperty] private string _currentPath;
    [ObservableProperty] private bool _canRedo;
    [ObservableProperty] private string _searchWatermark = string.Empty;
    [ObservableProperty] private bool _canUndo;
    [ObservableProperty] private bool _searcherPopupVisibility;

    #endregion
    
    #region Constructors 

    public MainWindowViewModel(
        IOptions<ExplorerOptions> explorerOptions,
        IMessenger messenger,
        IServiceScopeFactory scopeFactory,
        ViewModelLocator locator,
        StrongTypedLocalizationManager localizationManager) : base(localizationManager)
    {
        _messenger = messenger;
        _locator = locator;
        _scopeFactory = scopeFactory;
        
        _messenger.RegisterAll(this);
        ExplorerControl = _locator.GetView<ListExplorerItemsView>();
        
        _messenger.Send(new UpdateDirectoryContentRequestMessage
        {
            Directory = new(explorerOptions.Value.RootDirectory, IsDirectory: true)
        });
    }

    #endregion

    #region Receive methods

    public void Receive(ShowUnauthorizedAccessMessage message)
    {
        
    }

    public void Receive(UpdateCurrentDirectoryMessage message)
    {
        CurrentPath = message.Value;
    }

    public void Receive(GetCurrentDirectoryPathMessage message)
    {
        message.Reply(CurrentPath);
    }

    #endregion

    #region Relay commands

    [RelayCommand]
    private async Task UndoAsync()
    {
        var task = _messenger.Send<UndoFileEntityFromHistory>();
        SetCurrentPath(await task.Response);
    }
    
    [RelayCommand]
    private async Task RedoAsync()
    {
        var task = _messenger.Send<RedoFileEntityFromHistory>();
        SetCurrentPath(await task.Response);
    }

    [RelayCommand]
    private async Task StartShowingSearchWatermark()
    {
        CurrentDirectoryBeforeSearching = CurrentPath;
        _directorySearcherScope = _scopeFactory.CreateScope();
        await ChangeSearchWatermark();
    }
    
    [RelayCommand]
    private void StopShowingSearchWatermark()
    {
        CurrentPath = CurrentDirectoryBeforeSearching ?? string.Empty;
        _directorySearcherScope.Dispose();
        SearchWatermark = string.Empty;
        CurrentDirectoryBeforeSearching = null;
    }
    
    [RelayCommand]
    private async Task ChangeSearchWatermark()
    {
        await Task.Run(() =>
        {
            var searcher = _directorySearcherScope.ServiceProvider.GetRequiredService<IDirectorySearcher>();
            var entity = searcher.GetDirectory(CurrentDirectoryBeforeSearching ?? CurrentPath, CurrentPath);
            
            Application
                .Current
                .Dispatcher
                .Invoke(() => SearchWatermark = entity.Path);
        });
     
    }
    
    [RelayCommand]
    private async Task SetSearchWatermarkAsync()
    {
        CurrentDirectoryBeforeSearching = SearchWatermark;
        _messenger.Send(new UpdateDirectoryContentRequestMessage
        {
            Directory = new(SearchWatermark, IsDirectory: true)
        });
        
        await ChangeSearchWatermark();
    }
    
    #endregion

    #region Private methods

    private void SetCurrentPath(FileEntity? entity)
    {
        if (entity is null)
            return;
        
        CurrentPath = entity.Value.Path;
        _messenger.Send(new UpdateDirectoryContentRequestMessage { Directory = entity.Value });
    }
    
    #endregion

    #region ComminityToolkit overrides

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentPath) && CurrentDirectoryBeforeSearching is null)
        {
            CanRedo = _messenger.Send<CanRedoMessage>().Response;
            CanUndo = _messenger.Send<CanUndoMessage>().Response;
        }
        base.OnPropertyChanged(e);
    }

    #endregion
}