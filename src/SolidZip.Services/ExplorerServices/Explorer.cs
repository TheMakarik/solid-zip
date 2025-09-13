namespace SolidZip.Services.ExplorerServices;

internal sealed class Explorer(
    ILogger<Explorer> logger, 
    IOptions<ExplorerOptions> explorerOptions, 
    IDirectoryProxy directoryProxy) : IExplorer
{
    private const string LogicalDriveIsNotExistsLogMessage = "Logical drive: {drive} is not exists but was loaded, user possible has linux distro in dual-boot";
    private const string GotDirectoryContentLogMessage = "Got {directory} content for {times} milliseconds";
    private const string LoadingAdditionalContentForLogicalDrives = "Loading additional content {path} for logical drives";
    
    public (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity entity)
    {

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var result = GetDirectoryContentWithoutTimer(entity);
        
        stopwatch.Stop();
        logger.LogDebug(GotDirectoryContentLogMessage, entity.Path, stopwatch.ElapsedMilliseconds);
        return result;

    }

    private bool IsRootDirectory(string directoryPath)
    {
       return directoryPath == explorerOptions.Value.RootDirectory;
    }

    private IEnumerable<FileEntity> GetLogicalDrivesLazy()
    {
        foreach (var drive in directoryProxy.GetLogicalDrives() )
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

    private IEnumerable<FileEntity> AddAdditionalLogicalDrivesContent()
    {
        foreach (var content in explorerOptions.Value.RootDirectoryAdditionalContent)
        {
            var path = Environment.ExpandEnvironmentVariables(content);
            logger.LogDebug(LoadingAdditionalContentForLogicalDrives, path);
            yield return new FileEntity(content, IsDirectory: true);
        }
          
        
    }

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
                return (GetLogicalDrivesLazy(), ExplorerResult.Success);
            return (GetDirectoryContentLazy(entity.Path), ExplorerResult.Success);

        }
        catch (UnauthorizedAccessException exception)
        {
            return ([], ExplorerResult.UnauthorizedAccess);
        }
    }

    private bool IsLinuxLogicalDrive(string drive) => !directoryProxy.Exists(drive);

}