namespace SolidZip.Modules.AppData;


public sealed class UserJsonManager(ILogger<UserJsonManager> logger, SharedCache<UserData> cache, PathsCollection paths) : IUserJsonManager
{
    public void ChangeThemeName(string newThemeName)
    {
        cache.Value.CurrentTheme = newThemeName;
    }

    public void ChangeCurrentCulture(CultureInfo newCurrentCulture)
    {
        cache.Value.CurrentCulture = newCurrentCulture;
    }

    public void ChangeAttachPluginsConsole(bool attach)
    {
        cache.Value.AttachPluginsConsole = attach;
    }

    public void ChangeExplorerElementsView(ExplorerElementsView newExplorerElementsView)
    {
        cache.Value.ExplorerElementsView = newExplorerElementsView;
    }

    public void ChangeFileSizeMeasurement(FileSizeMeasurement newFileSizeMeasurement)
    {
        cache.Value.FileSizeMeasurement = newFileSizeMeasurement;
    }

    public void ChangeRootDirectoryAdditionalContent(IEnumerable<string> newContent)
    {
        cache.Value.RootDirectoryAdditionalContent = newContent.ToList();
    }

    public void ChangeShowHiddenDirectories(bool showHidden)
    {
        cache.Value.ShowHiddenDirectories = showHidden;
    }

    public async ValueTask<CultureInfo> GetCurrentCultureAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.CurrentCulture;
    }

    public async ValueTask<bool> GetAttachPluginsConsoleAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.AttachPluginsConsole;
    }

    public async ValueTask<ExplorerElementsView> GetExplorerElementsViewAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.ExplorerElementsView;
    }

    public async ValueTask<string> GetThemeNameAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.CurrentTheme;
    }

    public async ValueTask<FileSizeMeasurement> GetFileSizeMeasurementAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.FileSizeMeasurement;
    }

    public async ValueTask<IEnumerable<string>> GetRootDirectoryAdditionalContentAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.RootDirectoryAdditionalContent;
    }

    public async ValueTask<bool> GetShowHiddenDirectoriesAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.ShowHiddenDirectories;
    }

    private async ValueTask EnsureCacheExistingAsync()
    {
        if (!cache.Exists())
            await LoadFromFileAsync();
    }
    
    private async Task LoadFromFileAsync()
    {
        await using var stream = new FileStream(paths.UserData, FileMode.Open);
        cache.Value = await JsonSerializer.DeserializeAsync<UserData>(stream,  UserDataSerializerContext.Default.Options)
                      ?? throw new InvalidDataException($"{paths.UserData} file is corrupted");
        
        logger.LogInformation("User data cache loaded from file: {file}", paths.UserData);
    }
}