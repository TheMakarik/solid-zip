namespace SolidZip.Services.AppDataServices;

public class AppDataContentManager(
    ILogger<AppDataContentManager> logger,
    IJsonSerializer serializer, 
    IOptions<AppDataOptions> options) : IAppDataContentManager
{
    private const string ChangingThemeNameLogMessage = "Changing theme from {Old} to {New}";
    private const string ChangingCurrentCultureLogMessage = "Changing language from {Old} to {New}";
    private const string ChangingExplorerElementsViewLogMessage = "Changing ExplorerElementsView from {Old} to {New}";
    private const string GetThemeNameLogMessage = "Getting theme with value: {Value}";
    private const string GetCurrentCultureLogMessage = "Getting language with value: {Value}";
    private const string GetExplorerElementsViewLogMessage = "Getting ExplorerElementsView with value: {Value}";
    private const string CacheHitLogMessage = "Cache hit for {Property}";
    private const string CacheMissLogMessage = "Cache miss for {Property}, loading from file";
    private const string UpdatingCacheLogMessage = "Updating cache for {Property}";
    private const string SavingAppDataLogMessage = "Saving AppData content to {FilePath}";
    private const string LoadingAppDataLogMessage = "Loading AppData content from {FilePath}";
    private const string DeserializationErrorLogMessage = "Error deserializing AppData content from {FilePath}";
    
    private readonly string _appDataFilePath = Environment.ExpandEnvironmentVariables(options.Value.DataJsonFilePath);
    private AppDataContent? _cache;
    private bool _isCacheLoaded;

    public async Task ChangeThemeNameAsync(string newThemeName)
    {
        await UpdateAppDataContentAsync(
            content => content with { ThemeName = newThemeName },
            oldContent => oldContent.ThemeName,
            newThemeName,
            ChangingThemeNameLogMessage,
            nameof(AppDataContent.ThemeName));
    }

    public async Task ChangeCurrentCultureAsync(CultureInfo newCurrentCulture)
    {
        await UpdateAppDataContentAsync(
            content => content with { CurrentCulture = newCurrentCulture },
            oldContent => oldContent.CurrentCulture.ToString(),
            newCurrentCulture.ToString(),
            ChangingCurrentCultureLogMessage,
            nameof(AppDataContent.CurrentCulture));
    }

    public async Task ChangeExplorerElementsViewAsync(ExplorerElementsView newExplorerElementsView)
    {
        await UpdateAppDataContentAsync(
            content => content with { ExplorerElementsView = newExplorerElementsView },
            oldContent => oldContent.ExplorerElementsView.ToString(),
            newExplorerElementsView.ToString(),
            ChangingExplorerElementsViewLogMessage,
            nameof(AppDataContent.ExplorerElementsView));
    }
    
    public async ValueTask<CultureInfo> GetCurrentCultureAsync()
    {
        return await GetCachedValueAsync(
            content => content.CurrentCulture,
            GetCurrentCultureLogMessage,
            nameof(AppDataContent.CurrentCulture));
    }
    
    public async ValueTask<string> GetThemeNameAsync()
    {
        return await GetCachedValueAsync(
            content => content.ThemeName,
            GetThemeNameLogMessage,
            nameof(AppDataContent.ThemeName));
    }

    public async ValueTask<ExplorerElementsView> GetExplorerElementsViewAsync()
    {
        return await GetCachedValueAsync(
            content => content.ExplorerElementsView,
            GetExplorerElementsViewLogMessage,
            nameof(AppDataContent.ExplorerElementsView));
    }

    private async Task UpdateAppDataContentAsync(
        Func<AppDataContent, AppDataContent> updateFunction,
        Func<AppDataContent, string> oldValueSelector,
        string newValue,
        string logMessage,
        string propertyName)
    {
        var content = await EnsureAppDataContentLoadedAsync();
        var oldValue = oldValueSelector(content);
        
        logger.LogDebug(logMessage, oldValue, newValue);
        
        var updatedContent = updateFunction(content);
        await SaveAppDataContentAsync(updatedContent);
        
        UpdateCache(updatedContent, propertyName);
    }

    private async ValueTask<T> GetCachedValueAsync<T>(
        Func<AppDataContent, T> valueSelector,
        string logMessage,
        string propertyName)
    {
        if (_isCacheLoaded && _cache is not null)
        {
            logger.LogDebug(CacheHitLogMessage, propertyName);
            return valueSelector(_cache.Value);
        }

        logger.LogDebug(CacheMissLogMessage, propertyName);
        var content = await EnsureAppDataContentLoadedAsync();
        
        var value = valueSelector(content);
        logger.LogDebug(logMessage, value?.ToString());
        return value;
    }

    private async Task<AppDataContent> EnsureAppDataContentLoadedAsync()
    {
        if (_isCacheLoaded && _cache is not null)
        {
            return _cache.Value;
        }

        logger.LogDebug(LoadingAppDataLogMessage, _appDataFilePath);
        var content = await LoadAppDataContentAsync();
        
        _cache = content;
        _isCacheLoaded = true;
        return content;
    }

    private async Task<AppDataContent> LoadAppDataContentAsync()
    {
        logger.LogDebug(LoadingAppDataLogMessage, _appDataFilePath);
        return await serializer.DeserializeAsync<AppDataContent>(
                _appDataFilePath, 
                AppDataContentSerializerContext.Default.Options);
    }

    private async Task SaveAppDataContentAsync(AppDataContent content)
    {
     
        logger.LogDebug(SavingAppDataLogMessage, _appDataFilePath);
        await serializer.SerializeAsync(
            content, 
            _appDataFilePath, 
            AppDataContentSerializerContext.Default.Options);
    }

    private void UpdateCache(AppDataContent content, string propertyName)
    {
        _cache = content;
        logger.LogDebug(UpdatingCacheLogMessage, propertyName);
    }
}