namespace SolidZip.Services.ExplorerServices;

internal sealed class ExplorerFacade(IExplorer explorer, IServiceScopeFactory scopeFactory) : IExplorerFacade
{
    private ExplorerMode _mode = ExplorerMode.Directory;
    private IServiceScope? _searchScope;
    private IDirectorySearcher _searcher;
    
    public (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity entity)
    {
        throw new NotImplementedException();
    }

    public FileEntity Search(string path, string pattern)
    {
        throw new NotImplementedException();
    }

    public void SetMode(ExplorerMode mode)
    {
        _mode = mode;
    }

    public void StartSearching()
    {
        if (_mode == ExplorerMode.Directory)
            CreateDirectorySearcher();
          
    }

    public void StopSearching()
    {
        _searchScope.Dispose();
    }

    private void CreateDirectorySearcher()
    {
        _searchScope = scopeFactory.CreateScope();
        _searcher = _searchScope.ServiceProvider.GetRequiredService<IDirectorySearcher>();
    }
}