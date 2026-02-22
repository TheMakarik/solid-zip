namespace SolidZip.Modules.StateMachines;

public sealed class SearcherStateMachine(
    ILogger<SearcherStateMachine> logger,
    IServiceScopeFactory scopeFactory,
    IFileSystemStateMachine stateMachine,
    IOptions<ExplorerOptions> explorerOptions) : ISearcherStateMachine
{
    private IDirectorySearcher? _directorySearcher;
    private IServiceScope? _scope;

    public void Begin()
    {
        _scope = scopeFactory.CreateScope();
        if (stateMachine.GetState() == FileSystemState.Directory)
            _directorySearcher = _scope.ServiceProvider.GetService<IDirectorySearcher>();
    }

    public void End()
    {
        _scope?.Dispose();
        _directorySearcher = null;
    }

    public async ValueTask<FileEntity> Search(string path, string pattern)
    {
        if (stateMachine.GetState() != FileSystemState.Directory)
            return default;
        
        var foundPath = await _directorySearcher.Search(path, pattern) ?? string.Empty;

        if (!foundPath.StartsWith(explorerOptions.Value.RootDirectory))
            return foundPath.ToDirectoryFileEntity();

        if (foundPath.StartsWith(explorerOptions.Value.RootDirectory))
            return foundPath.Substring(explorerOptions.Value.RootDirectory.Length).ToDirectoryFileEntity() with
            {
                Path = foundPath
            };
        
        if (string.IsNullOrEmpty(path))
            return default(FileEntity) with { Path = string.Empty };
        return foundPath.ToDirectoryFileEntity(); //moves root prefix sz/ for loading directory info and backs it 

    }
}