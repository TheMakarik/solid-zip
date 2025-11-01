namespace SolidZip.Services.ExplorerServices;

internal sealed class DirectorySearcher(
    ILogger<DirectorySearcher> logger,
    IDirectoryProxy directory,
    IOptions<ExplorerOptions> explorerOptions) : IDirectorySearcher
{
    private const string FoundDirectoryLogMessage = "Found directory {Path} for the {Directory} by pattern {Pattern}";
    private const string SearchingInRootDirectoryLogMessage = "Searching in root directory with pattern: {Pattern}";
    private const string PatternNotFoundLogMessage = "Pattern not found in directory: {Directory}, pattern: {Pattern}";
    private const string PatternStartsWithDirectoryLogMessage = "Pattern starts with directory, trimming pattern. Original: {OriginalPattern}, Trimmed: {TrimmedPattern}";
    private const string ClearingFoundDirectories = "Clearing '_alreadyFoundDirectoreis', every directory may will be shown again, content before: {content}";
    private const string DirectorySearcherState = "DirectorySearcherState: AlreadyFoundDirectoreis: {directoreis}, LastPath: {path}, CurrentPath: {currentPath}";
    
    private string _lastPath = string.Empty;
    private readonly List<string> _alreadyFoundDirectories = new(5);

    public FileEntity Search(string path, string pattern)
    {
         logger.LogTrace(DirectorySearcherState, _alreadyFoundDirectories, _lastPath, path);
        
        if (path != _lastPath && _alreadyFoundDirectories.Any())
            ClearFoundDirectories();
        
        if (TryTrimPatternByExistingDirectory(ref pattern))
            logger.LogDebug(PatternStartsWithDirectoryLogMessage, path + pattern, pattern);

        
        if (path.StartsWith(explorerOptions.Value.RootDirectory))
            return GetDirectoryFromRoot(path, pattern);
        
        var foundDirectory = SearchDirectories(path, pattern)
            .FirstOrDefault(directoryPath => !_alreadyFoundDirectories.Contains(directoryPath) && directoryPath != path);
        
        if (foundDirectory is null)
        {
            logger.LogWarning(PatternNotFoundLogMessage, path, pattern);

            //Directory is possible empty, must to check it
            if (!directory.EnumerateDirectories(path).Any())
                return new FileEntity(path, IsDirectory: true);
            
            
            if (_alreadyFoundDirectories.Any())
            {
                ClearFoundDirectories();//Try clear and reshow
                Search(path, pattern);
            }
               
            
            return new FileEntity(string.Empty, IsDirectory: true);
        }

        logger.LogDebug(FoundDirectoryLogMessage, foundDirectory, path, pattern);
        _alreadyFoundDirectories.Add(foundDirectory);
        _lastPath = path;
        return new FileEntity(foundDirectory, IsDirectory: true);
    }

    private void ClearFoundDirectories()
    {
        logger.LogInformation(ClearingFoundDirectories, _alreadyFoundDirectories);
        _lastPath = string.Empty;
        _alreadyFoundDirectories.Clear();
    }

    private FileEntity GetDirectoryFromRoot(string path, string pattern)
    {
        var searchPattern = GetSearchPattern(path, explorerOptions.Value.RootDirectory);
        
        if (string.IsNullOrEmpty(searchPattern) || searchPattern == Path.DirectorySeparatorChar.ToString())
            return GetFirstRootDirectory();
        

        logger.LogTrace(SearchingInRootDirectoryLogMessage, searchPattern);
        
        foreach (var drive in directory.GetLogicalDrives())
        {
            var foundDirectory = SearchDirectories(drive, searchPattern)
                .FirstOrDefault(directoryPath => !_alreadyFoundDirectories.Contains(directoryPath));

            if (foundDirectory is null)
                continue;
            
            logger.LogDebug(FoundDirectoryLogMessage, foundDirectory, path, pattern);
            _alreadyFoundDirectories.Add(foundDirectory);
            return new FileEntity(explorerOptions.Value.RootDirectory + foundDirectory, IsDirectory: true);
        }

        logger.LogWarning(PatternNotFoundLogMessage, path, pattern);
        throw new DirectoryNotFoundException($"Directory not found in root for path: {path}, pattern: {pattern}");
    }

    private FileEntity GetFirstRootDirectory()
    {
        var firstDrive = directory.GetLogicalDrives().First();
        logger.LogDebug(FoundDirectoryLogMessage, firstDrive, explorerOptions.Value.RootDirectory, "root");
        return new FileEntity(firstDrive, IsDirectory: true);
    }

    private IEnumerable<string> SearchDirectories(string path, string pattern)
    {
        var searchPattern = string.IsNullOrEmpty(pattern) ? "*" : pattern + "*";
        
        return directory.EnumerateDirectories(path, searchPattern, new EnumerationOptions()
        {
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseSensitive,
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetSearchPattern(string fullPattern, string rootPattern)
    {
        return fullPattern.Length <= rootPattern.Length 
            ? string.Empty 
            : fullPattern[rootPattern.Length..];
    }

    private bool TryTrimPatternByExistingDirectory(ref string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return false;
        
        for (var i = pattern.Length; i > 0; i--)
        {
            var potentialDirectory = pattern[..i];
            
            if (!directory.Exists(potentialDirectory)) 
                continue;
            
            pattern = pattern[i..];
            return true;
        }

        return false;
    }
}
