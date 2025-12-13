namespace SolidZip.Modules.Explorer;

public sealed class DirectorySearcher(
    ILogger<DirectorySearcher> logger,
    IOptions<ExplorerOptions> explorerOptions) : IDirectorySearcher
{
    private string _lastPath = string.Empty;
    private readonly ConcurrentBag<string> _alreadyFoundDirectories = new();

    public string Search(string path, string pattern)
    {
        var mustAddRootDirectory = false;

        if (path.StartsWith(explorerOptions.Value.RootDirectory))
        {
            path = path.Substring(explorerOptions.Value.RootDirectory.Length);
            mustAddRootDirectory = true;
        }
          
       
        logger.LogTrace("DirectorySearcherState: AlreadyFoundDirectories: {directories}, LastPath: {path}, CurrentPath: {currentPath}", 
            _alreadyFoundDirectories, _lastPath, path);
        
        if (path != _lastPath && _alreadyFoundDirectories.Any())
            ClearFoundDirectories();
        
        if (TryTrimPatternByExistingDirectory(ref pattern))
            logger.LogDebug("Pattern starts with directory, trimming pattern. Original: {OriginalPattern}, Trimmed: {TrimmedPattern}", 
                path + pattern, pattern);

        
        if (string.IsNullOrEmpty(path))
            return SearchWithRootDirectory(() => GetDirectoryFromRoot(path, pattern),  mustAddRootDirectory);
        
        var foundDirectory = SearchDirectories(path, pattern)
            .FirstOrDefault(directoryPath => !_alreadyFoundDirectories.Contains(directoryPath) && directoryPath != path);
        
        if (foundDirectory is null)
        {
            logger.LogWarning("Pattern not found in directory: {Directory}, pattern: {Pattern}", path, pattern);

            // Directory is possible empty, must check it
            if (!Directory.EnumerateDirectories(path).Any())
                return path;


            if (!_alreadyFoundDirectories.Any()) 
                return string.Empty;
            
            
            ClearFoundDirectories(); // Try clear and reshow
            return SearchWithRootDirectory(() => Search(path, pattern),  mustAddRootDirectory);
        }

        logger.LogDebug("Found directory {Path} for the {Directory} by pattern {Pattern}", foundDirectory, path, pattern);
        _alreadyFoundDirectories.Add(foundDirectory);
        _lastPath = path;
        return mustAddRootDirectory 
            ? explorerOptions.Value.RootDirectory + foundDirectory 
            :  foundDirectory;
    }

    private void ClearFoundDirectories()
    {
        logger.LogInformation("Clearing '_alreadyFoundDirectories', every directory may will be shown again, content before: {content}", 
            _alreadyFoundDirectories);
        _lastPath = string.Empty;
        _alreadyFoundDirectories.Clear();
    }

    private string GetDirectoryFromRoot(string path, string pattern)
    {
        var searchPattern = GetSearchPattern(path, explorerOptions.Value.RootDirectory);
        
        if (string.IsNullOrEmpty(searchPattern) || searchPattern == Path.DirectorySeparatorChar.ToString())
            return GetFirstRootDirectory();
        
        foreach (var drive in Directory.GetLogicalDrives().Where(Directory.Exists)) //skip linux distro partitions
        {
            var foundDirectory = SearchDirectories(drive, searchPattern)
                .FirstOrDefault(directoryPath => !_alreadyFoundDirectories.Contains(directoryPath));

            if (foundDirectory is null)
                continue;
            
            logger.LogDebug("Found directory {Path} for the {Directory} by pattern {Pattern}", foundDirectory, path, pattern);
            _alreadyFoundDirectories.Add(foundDirectory);
            return explorerOptions.Value.RootDirectory + foundDirectory;
        }

        logger.LogWarning("Pattern not found in directory: {Directory}, pattern: {Pattern}", path, pattern);
        throw new DirectoryNotFoundException($"Directory not found in root for path: {path}, pattern: {pattern}");
    }

    private string GetFirstRootDirectory()
    {
        var firstDrive = Directory.GetLogicalDrives().First();
        logger.LogDebug("Found directory {Path} for the {Directory} by pattern {Pattern}", 
            firstDrive, explorerOptions.Value.RootDirectory, "root");
        return firstDrive;
    }

    private IEnumerable<string> SearchDirectories(string path, string pattern)
    {
        var searchPattern = string.IsNullOrEmpty(pattern) ? "*" : pattern + "*";
        
        return Directory.EnumerateDirectories(path, searchPattern, new EnumerationOptions()
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
            
            if (!Directory.Exists(potentialDirectory)) 
                continue;
            
            pattern = pattern[i..];
            return true;
        }

        return false;
    }

    private string SearchWithRootDirectory(Func<string> action, bool mustAddRootDirectory)
    {
        return mustAddRootDirectory 
            ? explorerOptions.Value.RootDirectory + action() 
            : action();
    }
}