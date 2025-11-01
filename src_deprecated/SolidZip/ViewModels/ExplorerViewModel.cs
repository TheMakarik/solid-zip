namespace SolidZip.ViewModels;

public sealed partial class ExplorerViewModel 
    : ViewModelBase, IRecipient<UpdateDirectoryContentRequestMessage>, 
        IRecipient<RedoFileEntityFromHistory>, 
        IRecipient<UndoFileEntityFromHistory>, 
        IRecipient<CanRedoMessage>,
        IRecipient<CanUndoMessage>
{

    #region Consts
    
    private const string ChangingCanUndoLogMessage = "Changed CanUndo to {value}";
    private const string ChangingCanRedoLogMessage = "Changed CanRedo to {value}";
    
    #endregion
 
    #region Services

    private readonly ILogger<ExplorerViewModel> _logger;
    private readonly IExplorer _explorer;
    private readonly IMessenger _messenger;
    private readonly IOptions<ExplorerOptions> _explorerOptions;
    private readonly IExplorerHistory _explorerHistory;
    private readonly IPathProxy _pathProxy;
    private readonly IDirectoryProxy _directory;

    #endregion
    
    #region Properties for view

    [ObservableProperty] private ObservableCollection<FileEntity> _entities = new();
    [ObservableProperty] private ObservableCollection<FileEntity> _selectedEntities;
    
    #endregion
    
    #region Constructors 

    public ExplorerViewModel(
        IPathProxy pathProxy,
        IDirectoryProxy directory,
        IExplorerHistory explorerHistory,
        IExplorer explorer, 
        IMessenger messenger,
        IOptions<ExplorerOptions> explorerOptions,
        ILogger<ExplorerViewModel> logger,
        StrongTypedLocalizationManager localizationManager) : base(localizationManager)
    {
        _explorer = explorer;
        _messenger = messenger;
        _logger = logger;
        _directory = directory;
        _explorerOptions = explorerOptions;
        _explorerHistory = explorerHistory;
        _pathProxy = pathProxy;
        
        _messenger.RegisterAll(this);
    }

    #endregion
    
    #region Recive methods 

    public void Receive(UpdateDirectoryContentRequestMessage message)
    {
        //Do not invoke default UpdateDirectoryContent() method because it won't add entity to Explorer History
        UpdateDirectoryContentCommand.Execute(message.Directory);
    }
    
    public void Receive(RedoFileEntityFromHistory message)
    {
        _explorerHistory.Redo();
        ReplyCurrentEntity(message);
    }
    public void Receive(UndoFileEntityFromHistory message)
    {
        _explorerHistory.Undo();
        ReplyCurrentEntity(message);
    }
    
    public void Receive(CanRedoMessage message)
    {
        message.Reply(_explorerHistory.CanRedo);
        _logger.LogDebug(ChangingCanRedoLogMessage, _explorerHistory.CanRedo);
    }

    public void Receive(CanUndoMessage message)
    {
        message.Reply(_explorerHistory.CanUndo);
        _logger.LogDebug(ChangingCanUndoLogMessage, _explorerHistory.CanUndo);
    }

    #endregion

    #region Relay commands
    
    [RelayCommand] 
    private void UpdateDirectoryContent(object entity)
    {
        //I don't know why but after adding ItemDoubleClickBehavior randomly
        //ExplorerView returns string instead of FileEntity
        var directory = entity is string path
            ? new FileEntity(path, _directory.Exists(path), IsArchiveEntry: false)
            : (FileEntity)entity;
        
        UpdateDirectoryContentWithoutHistory(directory);
        
        _explorerHistory.CurrentEntity = directory;
    }
    
    #endregion

    #region Private methods

    private void UpdateDirectoryContentWithoutHistory(FileEntity entity)
    {
        entity.Path = GetRealDirectoryName(entity) ?? string.Empty;
        var result = _explorer.GetDirectoryContent(entity);
        FillDirectoryContent(result.Entities, entity.Path);
        
        _messenger.Send(new UpdateCurrentDirectoryMessage(entity.Path));
    }
    
    private string? GetRealDirectoryName(FileEntity entity)
    {
        if (entity.Path == _explorerOptions.Value.DeeperDirectoryName)
            return _pathProxy.GetDirectoryName(
                _messenger.Send(new GetCurrentDirectoryPathMessage()).Response);
        return entity.Path;
    }
    

    private void FillDirectoryContent(IEnumerable<FileEntity> values, string path)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            Entities.Clear();
            AddDeeperDirectory(path);
            foreach (var entity in values)
                Entities.Add(entity);
        });
    }
    
    private void AddDeeperDirectory(string path)
    {
        if(path != _explorerOptions.Value.RootDirectory)
            Entities.Add(new FileEntity(_explorerOptions.Value.DeeperDirectoryName, IsDirectory: true));
    }

    private void ReplyCurrentEntity(AsyncRequestMessage<FileEntity?> message)
    {
        var result = _explorerHistory.CurrentEntity;
        message.Reply(result); 
        UpdateDirectoryContent(result);
    }

    #endregion
    
}