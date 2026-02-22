namespace SolidZip.Modules.AppData;

public sealed class UserJsonManager(ILogger<UserJsonManager> logger, SharedCache<UserData> cache, PathsCollection paths)
    : IUserJsonManager
{
    public void ChangeThemeName(string newThemeName)
    {
       cache.SetValueProperty((self, v) => self.CurrentTheme = v, newThemeName);
    }

    public void ChangeCurrentCulture(CultureInfo newCurrentCulture)
    {
        cache.Value.CurrentCulture = newCurrentCulture;
        cache.WasChanged = true;
    }

    public void ChangeAttachPluginsConsole(bool attach)
    {
        cache.Value.AttachPluginsConsole = attach;
        cache.WasChanged = true;
    }

    public void ChangeExplorerElementsView(string newExplorerElementsView)
    {
        cache.Value.ExplorerElementsView = newExplorerElementsView;
        cache.WasChanged = true;
    }

    public void ChangeFileSizeMeasurement(FileSizeMeasurement newFileSizeMeasurement)
    {
        cache.Value.FileSizeMeasurement = newFileSizeMeasurement;
        cache.WasChanged = true;
    }

    public void ChangeRootDirectoryAdditionalContent(IEnumerable<string> newContent)
    {
        cache.Value.RootDirectoryAdditionalContent = newContent.ToList();
        cache.WasChanged = true;
    }

    public void ChangeShowHiddenDirectories(bool showHidden)
    {
        cache.Value.ShowHiddenDirectories = showHidden;
        cache.WasChanged = true;
    }

    public void ChangeExplorerElementsHeight(int newExplorerElementsHeight)
    {
        cache.Value.ExplorerElementsHeight = newExplorerElementsHeight;
        cache.WasChanged = true;
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

    public async ValueTask<string> GetExplorerElementsViewAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.ExplorerElementsView;
    }

    public async ValueTask<string> GetThemeNameAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.CurrentTheme;
    }

    public async ValueTask<int> GetExplorerElementsHeightAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value.ExplorerElementsHeight;
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

    public async ValueTask<UserData> GetAllAsync()
    {
        await EnsureCacheExistingAsync();
        return cache.Value;
    }

    public void ChangeAll(UserData userData)
    {
        cache.Value = userData;
        //cache.WasChanged = true is not needed
    }

    public void ExpandChanges()
    {
        cache.ExpandChanges();
    }

    public async ValueTask EnsureCacheExistingAsync()
    {
        if (!cache.Exists())
            await LoadFromFileAsync();
    }

    private async Task LoadFromFileAsync()
    {
        await using var stream = new FileStream(paths.UserData, FileMode.Open);
        cache.Value = await JsonSerializer.DeserializeAsync<UserData>(stream, UserDataSerializerContext.Default.Options)
                      ?? throw new InvalidDataException($"{paths.UserData} file is corrupted");

        logger.LogInformation("User data cache loaded from file: {file}", paths.UserData);
    }
}