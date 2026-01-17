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
    private FileSystemState _state = FileSystemState.Directory;
    private IArchiveReader? _archiveReader;
    private IDirectorySearcher? _directorySearcher;
    private IArchiveDirectorySearcher?  _archiveDirectorySearcher;
    private string _currentPath;
    private IServiceScope? _scope;

    public bool CanUndo => explorerHistory.CanUndo;
    public bool CanRedo => explorerHistory.CanRedo;
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory)
    {
        directory = directory with { Path = Environment.ExpandEnvironmentVariables(directory.Path) };
        var result = _state == FileSystemState.Directory 
            ? await explorer.GetDirectoryContentAsync(directory)
            : _archiveReader!.GetEntries(directory);
        _currentPath =  directory.Path;
        TryToUpdateState(directory.Path);
        
        return result;
    }
    
    public IconInfo GetIcon(string path, FileSystemState? state = null)
    {
        return (state ?? _state) == FileSystemState.Directory 
            ? iconExtractor.Extract(path) 
            : archiveContentIconExtractor.Extract(path.GetExtensionFromEnd());
    }

    public FileEntity Redo()
    { 
        explorerHistory.Redo();
        var result =  explorerHistory.CurrentEntity;
        _currentPath = result.Path;
        return result;
    }

    public void AddToHistory(FileEntity entity)
    {
        explorerHistory.CurrentEntity = entity;
    }

    public FileEntity Undo()
    {
        explorerHistory.Undo();
        var result =  explorerHistory.CurrentEntity;
        _currentPath = result.Path;
        return result;
    }

    public void BeginSearch()
    {
        _scope = scopeFactory.CreateScope();
        if(_state == FileSystemState.Directory)
            _directorySearcher = _scope.ServiceProvider.GetRequiredService<IDirectorySearcher>();
        else
            _archiveDirectorySearcher = _scope.ServiceProvider.GetRequiredService<IArchiveDirectorySearcher>();
        logger.LogInformation("Start searching with state: {State}", _state);
    }

    public FileEntity Search(string path, string pattern)
    {
        if(_state == FileSystemState.Archive)
            return _archiveDirectorySearcher!.Search(path, pattern,  _currentPath, _archiveReader);
         
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

    public bool CanCreateItemHere()
    {
        return _currentPath != options.Value.RootDirectory;
    }

    public void CreateDirectory(string name)
    {
        if (_state == FileSystemState.Directory)
        {
            var path = Path.Combine(_currentPath, name);
            Directory.CreateDirectory(path);
            logger.LogInformation("Created folder: {Path}", path);
        }
          
    }

    private void TryToUpdateState(string path)
    {
        if (CanChangeStateToArchive(path, out var result))
        {
            _archiveReader = result;
            _currentPath = path;
            _state = FileSystemState.Archive;
        }

        if (CanChangeStateToDirectory(path))
            _state = FileSystemState.Archive;
        if(_state == FileSystemState.Directory)
            _archiveReader?.Dispose();
    }

    private bool CanChangeStateToDirectory(string path)
    {
        return _state == FileSystemState.Archive && Directory.Exists(path);
    }

    private bool CanChangeStateToArchive(string path, out IArchiveReader? result)
    {
        IArchiveReader? reader = null;
        var canChange =   ((!Directory.Exists(path) 
                            || _state == FileSystemState.Directory)
                            && factory.TryGetFactory(path.CutFromEnd(Path.DirectorySeparatorChar, '.'), out reader));
        result = reader;
        return canChange;
    }
    
}