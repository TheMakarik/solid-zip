namespace SolidZip.Modules.StateMachines;

public class ExplorerStateMachine(
    IExplorer explorer, 
    AssociatedIconExtractor iconExtractor,
    ExtensionIconExtractor archiveContentIconExtractor,
    IExplorerHistory explorerHistory, 
    IOptions<ExplorerOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<ExplorerStateMachine> logger,
    ArchiveReaderFactory factory) : IExplorerStateMachine
{
    private ExplorerState _state = ExplorerState.Directory;
    private IArchiveReader? _archiveReader;
    private IDirectorySearcher? _directorySearcher;
    private IArchiveDirectorySearcher?  _archiveDirectorySearcher;
    private string? _archivePath;
    private IServiceScope? _scope;

    public bool CanUndo => explorerHistory.CanUndo;
    public bool CanRedo => explorerHistory.CanRedo;
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory, bool addToHistory = true)
    {
        var result = _state == ExplorerState.Directory 
            ? await explorer.GetDirectoryContentAsync(directory)
            : _archiveReader!.GetEntries(directory);
        TryToUpdateState(directory.Path);
        
        if(addToHistory)
             explorerHistory.CurrentEntity = directory;
        return result;
    }
    
    public IconInfo GetIcon(string path, ExplorerState? state = null)
    {
        return (state ?? _state) == ExplorerState.Directory 
            ? iconExtractor.Extract(path) 
            : archiveContentIconExtractor.Extract(path.GetExtensionFromEnd());
    }

    public FileEntity Redo()
    { 
        explorerHistory.Redo();
        return explorerHistory.CurrentEntity;
    }

    public FileEntity Undo()
    {
        explorerHistory.Undo();
        return explorerHistory.CurrentEntity;
    }

    public void BeginSearch()
    {
        _scope = scopeFactory.CreateScope();
        if(_state == ExplorerState.Directory)
            _directorySearcher = _scope.ServiceProvider.GetRequiredService<IDirectorySearcher>();
        else
            _archiveDirectorySearcher = _scope.ServiceProvider.GetRequiredService<IArchiveDirectorySearcher>();
        logger.LogInformation("Start searching with state: {State}", _state);
    }

    public FileEntity Search(string path, string pattern)
    {
        if(_state == ExplorerState.Archive)
            return _archiveDirectorySearcher!.Search(path, pattern,  _archivePath, _archiveReader);
         
        var result = _directorySearcher!.Search(path, pattern);
        if(result.StartsWith(options.Value.RootDirectory))
            return result.Substring(options.Value.RootDirectory.Length).ToDirectoryFileEntity() with { Path = result};
        if (string.IsNullOrEmpty(path))
            return default(FileEntity) with {Path = string.Empty};
        return result.ToDirectoryFileEntity();
    }

    public void EndSearch()
    {
        _scope?.Dispose();
        _scope = null;
    }

    private void TryToUpdateState(string path)
    {
        if (CanChangeStateToArchive(path, out var result))
        {
            _archiveReader = result;
            _archivePath = path;
            _state = ExplorerState.Archive;
        }

        if (CanChangeStateToDirectory(path))
            _state = ExplorerState.Archive;
        if(_state == ExplorerState.Directory)
            _archiveReader?.Dispose();
    }

    private bool CanChangeStateToDirectory(string path)
    {
        return _state == ExplorerState.Archive && Directory.Exists(path);
    }

    private bool CanChangeStateToArchive(string path, out IArchiveReader? result)
    {
        IArchiveReader? reader = null;
        var canChange =   ((!Directory.Exists(path) 
                            || _state == ExplorerState.Directory)
                            && factory.TryGetFactory(path.CutFromEnd(Path.DirectorySeparatorChar, '.'), out reader));
        result = reader;
        return canChange;
    }
    
}