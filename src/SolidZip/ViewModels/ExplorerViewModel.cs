namespace SolidZip.ViewModels;

public sealed partial class ExplorerViewModel 
    : ViewModelBase, IRecipient<UpdateDirectoryContentRequestMessage>, 
        IRecipient<RedoFileEntityFromHistory>, 
        IRecipient<UndoFileEntityFromHistory>, 
        IRecipient<CanRedoMessage>,
        IRecipient<CanUndoMessage>
{

    #region Consts

    private const string GettingDirectoryContentResultCountLogMessage = "Getting content from {path} count: {count} and result {result}";
    private const string GettingDirectoryContentValues = "Getting content from {path}: {values} and result {result}";
    private const string ChangingCanUndoLogMessage = "Changed CanUndo to {value}";
    private const string ChangingCanRedoLogMessage = "Changed CanRedo to {value}";

    private const string LoadedEvent = "EXPLORERVIEWMODEL_LOADED";
    private const string UpdateDirectoryContentEvent = "EXPLORERVIEWMODEL_UpdateDirectoryContent";
    private const string Event = "EXPLORERVIEWMODEL_";
    private const string UndoEvent = "EXPLORERVIEWMODEL_Undo";
    private const string RedoEvent = "EXPLORERVIEWMODEL_REDO";
    
    #endregion
 
    #region Services

    private readonly ILogger<ExplorerViewModel> _logger;
    private readonly IExplorer _explorer;
    private readonly IMessenger _messenger;
    private readonly IOptions<ExplorerOptions> _explorerOptions;
    private readonly IExplorerHistory _explorerHistory;
    private readonly IPathProxy _pathProxy;
    private readonly ILuaExtensionsRaiser _luaExtensionsRaiser;

    #endregion
    
    #region Properties for view

    [ObservableProperty] private ObservableCollection<FileEntity> _entities = new();
    [ObservableProperty] private FileEntity _selectedEntity;

    #endregion
    
    #region Constructors 

    public ExplorerViewModel(
        IPathProxy pathProxy,
        IExplorerHistory explorerHistory,
        IExplorer explorer, 
        IMessenger messenger,
        ILuaExtensionsRaiser luaExtensionsRaiser,
        IOptions<ExplorerOptions> explorerOptions,
        ILogger<ExplorerViewModel> logger,
        StrongTypedLocalizationManager localizationManager) : base(localizationManager)
    {
        _explorer = explorer;
        _messenger = messenger;
        _logger = logger;
        _luaExtensionsRaiser = luaExtensionsRaiser;
        _explorerOptions = explorerOptions;
        _explorerHistory = explorerHistory;
        _pathProxy = pathProxy;
        
        _messenger.RegisterAll(this);
        
        Task.Run(() =>
        {
            var rootDirectoryContent = GetDirectoryContent(
                new FileEntity(_explorerOptions.Value.RootDirectory, IsDirectory: true));
            
            rootDirectoryContent.Entities = rootDirectoryContent.Entities.Select(d =>
                new FileEntity(Environment.ExpandEnvironmentVariables(d.Path), true));
            FillDirectoryContent(rootDirectoryContent.Entities, explorerOptions.Value.RootDirectory);
        });
    }

    #endregion
    
    #region Recive methods 

    public void Receive(UpdateDirectoryContentRequestMessage message)
    {
        //Do not invoke default UpdateDirectoryContent() method because it won't add entity to Explorer History
        UpdateDirectoryContentCommand.Execute(message.Directory);
    }
    
    //This two receive methods invokes UpdateDirectoryContent from ReplyCurrentEntity,
    //you do not need to invoke it from other viewModels
    public void Receive(RedoFileEntityFromHistory message)
    {
        if(!_explorerHistory.CanRedo)
            message.Reply((FileEntity?)null);
        _explorerHistory.Redo();
        ReplyCurrentEntity(message);
    }
    public void Receive(UndoFileEntityFromHistory message)
    {
        if (!_explorerHistory.CanUndo)
            message.Reply((FileEntity?)null);
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
    private void UpdateDirectoryContent(FileEntity entity)
    {
        entity.Path = GetRealDirectoryName(entity) ?? string.Empty;
        var result = _explorer.GetDirectoryContent(entity);
        FillDirectoryContent(result.Entities, entity.Path);
        
        _messenger.Send(new UpdateCurrentDirectoryMessage(entity.Path));
        
        _explorerHistory.CurrentEntity = entity;
    }

    #endregion

    #region Private methods

    private string? GetRealDirectoryName(FileEntity entity)
    {
        if (entity.Path == _explorerOptions.Value.DeeperDirectoryName)
            return _pathProxy.GetDirectoryName(
                _messenger.Send(new GetCurrentDirectoryPathMessage()).Response);
        return entity.Path;
    }
    
    private (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity directory)
    {
        var result =  _explorer.GetDirectoryContent(directory);
        LogDirectoryContentCount(directory, result);
        _logger.LogDebug(GettingDirectoryContentValues, directory.Path, result.Entities, result.Result);
        return result;
    }

    private void LogDirectoryContentCount(FileEntity directory, (IEnumerable<FileEntity> Entities, ExplorerResult Result) result)
    {
        var counter = 0;
        result.Entities.ForEach(el => counter++);
        _logger.LogInformation(GettingDirectoryContentResultCountLogMessage, directory.Path, counter, result.Result);
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
    
    #region Private validation methods

    private bool IsLogicalDrive(string path)
    {
        return path is [_, ':', '\\'];
    }

    #endregion
}