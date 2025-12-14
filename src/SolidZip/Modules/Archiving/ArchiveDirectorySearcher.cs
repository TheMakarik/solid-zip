namespace SolidZip.Modules.Archiving;

public class ArchiveDirectorySearcher(ILogger<ArchiveDirectorySearcher> logger, IOptions<ExplorerOptions> explorerOptions) : IArchiveDirectorySearcher
{
    private string _lastPath = string.Empty;
    private readonly ConcurrentBag<FileEntity> _alreadyFoundDirectories = new();

    public FileEntity Search(string path, string pattern, string archivePath, IArchiveReader reader)
    {
        var mustAddRootDirectory = false;

        if (path.StartsWith(explorerOptions.Value.RootDirectory))
        {
            path = path.Substring(explorerOptions.Value.RootDirectory.Length);
            mustAddRootDirectory = true;
        }


        return default;
    }
    
    private string SearchWithRootDirectory(Func<string> action, bool mustAddRootDirectory)
    {
        return mustAddRootDirectory 
            ? explorerOptions.Value.RootDirectory + action() 
            : action();
    }
}