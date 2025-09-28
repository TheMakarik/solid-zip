namespace SolidZip.Services.ExplorerServices;

internal sealed class Explorer(
    ILogger<Explorer> logger, 
    IOptions<ExplorerOptions> explorerOptions, 
    IDirectoryProxy directoryProxy) : IExplorer
{
    private const string LogicalDriveIsNotExistsLogMessage = "Logical drive: {drive} is not exists but was loaded, user possible has linux distro in dual-boot";
    private const string GotDirectoryContentLogMessage = "Got {directory} content for {times} nanoseconds";
    private const string LoadingAdditionalContentForLogicalDrives = "Loading additional content {path} for logical drives";
    private const string DirectoryDoesNotExistLogMessage = "Directory {path} does not exists";
    
    public (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity entity)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = GetDirectoryContentWithoutTimer(entity);
        stopwatch.Stop();
        
        logger.LogDebug(GotDirectoryContentLogMessage, entity.Path, stopwatch.Elapsed.Nanoseconds);
        return result;
    }

    private bool IsRootDirectory(string directoryPath) => 
        directoryPath == explorerOptions.Value.RootDirectory;

    private IEnumerable<FileEntity> GetLogicalDrives()
    {
        foreach (var drive in directoryProxy.GetLogicalDrives())
        {
            if (IsLinuxLogicalDrive(drive))
            {
                logger.LogError(LogicalDriveIsNotExistsLogMessage, drive);
                continue;
            }
            
            yield return new FileEntity(drive, IsDirectory: true);
        }

        foreach (var fileEntity in AddAdditionalLogicalDrivesContent())
            yield return fileEntity;
    }

    private IEnumerable<FileEntity> AddAdditionalLogicalDrivesContent() => 
        explorerOptions.Value.RootDirectoryAdditionalContent
            .Select(content => 
            {
                var path = Environment.ExpandEnvironmentVariables(content);
                logger.LogDebug(LoadingAdditionalContentForLogicalDrives, path);
                return new FileEntity(path, IsDirectory: true);
            });

    private IEnumerable<FileEntity> GetDirectoryContentLazy(string path)
    {
        foreach (var directory in directoryProxy.EnumerableDirectories(path))
            yield return new FileEntity(directory, IsDirectory: true);
            
        foreach (var file in directoryProxy.EnumerateFiles(path))
            yield return new FileEntity(file, IsDirectory: false);
    }

    private (IEnumerable<FileEntity>, ExplorerResult) GetDirectoryContentWithoutTimer(FileEntity entity)
    {
        if (!entity.IsDirectory)
            return ([], ExplorerResult.NotDirectory);
        
        try
        {
            if (IsRootDirectory(entity.Path))
                return (GetLogicalDrives(), ExplorerResult.Success);
                
            if (!directoryProxy.Exists(entity.Path))
            {
                logger.LogError(DirectoryDoesNotExistLogMessage, entity.Path); //user renamed directory that shown in explorer view and try to open it
                return ([], ExplorerResult.UnexistingDirectory);
            }
            
            return (GetDirectoryContentLazy(entity.Path), ExplorerResult.Success);
        }
        catch (UnauthorizedAccessException)
        {
            return ([], ExplorerResult.UnauthorizedAccess);
        }
    }

    private bool IsLinuxLogicalDrive(string drive) => !directoryProxy.Exists(drive);
}