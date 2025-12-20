namespace SolidZip.Modules.Explorer;

public sealed class Explorer(ILogger<Explorer> logger, IUserJsonManager userJson, IOptions<ExplorerOptions> explorerOptions) : IExplorer
{
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetDirectoryContentAsync(FileEntity directory)
    {
        if (!directory.IsDirectory)
        {
            logger.LogWarning("{path} is not directory, cannot open it", directory.Path);
            return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.NotDirectory);
        }

        if (directory.Path == explorerOptions.Value.RootDirectory)
            return await GetRootContentAsync();

        if(directory.Path.StartsWith(explorerOptions.Value.RootDirectory))
            directory = directory with {Path = directory.Path.Substring(explorerOptions.Value.RootDirectory.Length)};
        
        if (!Directory.Exists(directory.Path))
            return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.UnexistingDirectory);
        
        try
        {
            var result = Directory.EnumerateDirectories(directory.Path)
                .Select(dir => dir.ToDirectoryFileEntity())
                .Concat(Directory.EnumerateFiles(directory.Path)
                    .Select(path => path.ToFileEntity()));
            logger.LogDebug("Directory content: {content}", result);
            
            //PROBLEM: https://stackoverflow.com/questions/79777381/unauthorizedaccessexception-not-being-caught-in-fakeiteasy-test
            result.Any();

            return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, result);
        }
        catch (UnauthorizedAccessException e)
        {
            return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.UnauthorizedAccess);
        }
    }

    private async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetRootContentAsync()
    {
        var drives = Directory
            .GetLogicalDrives()
            .Select(drive => drive.ToDirectoryFileEntity())
            .ToArray();

        drives = SkipLinuxLogicalPartitions(drives).ToArray();
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success,
            drives.Concat(await LoadAdditionalRootContentAsync()));
        
    }

    private async Task<IEnumerable<FileEntity>> LoadAdditionalRootContentAsync()
    {
        var additionalContent = await userJson.GetRootDirectoryAdditionalContentAsync();
        var result = additionalContent.Select(content => content.ToDirectoryFileEntity());
        logger.LogInformation("Loaded additional content for root: {content}", result);
        return result;
    }

    private IEnumerable<FileEntity> SkipLinuxLogicalPartitions(FileEntity[] drives)
    {
        foreach (var drive in drives)
        {
            if (Directory.Exists(drive.Path))
                yield return drive;
            else
               logger.LogWarning("{path} is not existing logical drive, maybe it's linux  partition and user has dual boot with linux distro", drive.Path);
        }
    }
}