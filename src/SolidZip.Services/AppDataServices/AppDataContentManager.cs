namespace SolidZip.Services.AppDataServices;

public class AppDataContentManager(
    ILogger<AppDataContentManager> logger,
    IJsonSerializer serializer, 
    IOptions<AppDataOptions> options) : IAppDataContentManager
{
    private const string ChangingThemeNameLogMessage = "Chaging theme from {old} to {new}";
    private const string ChangingCurrentCultureLogMessage = "Chaging language from {old} to {new}";
    private const string ChangingExplorerElementsViewLogMessage = "Chaging ExplorerElementsView from {old} to {new}";
    private const string GetThemeNameLogMessage = "Getting theme with value: {value}";
    private const string GetCurrentCultureLogMessage = "Getting language with value: {value}";
    private const string GetExplorerElementsViewLogMessage = "Getting  ExplorerElementsView with value: {value}";
    
    private readonly string _appDataFilePath = Environment.ExpandEnvironmentVariables(options.Value.DataJsonFilePath);
    
    public async Task ChangeThemeNameAsync(string newThemeName)
    {
       var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
       logger.LogDebug(ChangingThemeNameLogMessage, result.ThemeName, newThemeName);
       result.ThemeName = newThemeName;
       await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task ChangeCurrentCultureAsync(CultureInfo newCurrentCulture)
    {
        var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
        logger.LogDebug(ChangingCurrentCultureLogMessage, result.CurrentCulture.ToString(), newCurrentCulture.ToString());
        result.CurrentCulture = newCurrentCulture;
        await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task ChangeExplorerElementsView(ExplorerElementsView newExplorerElementsView)
    {
        var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
        logger.LogDebug(ChangingExplorerElementsViewLogMessage, result.ExplorerElementsView, newExplorerElementsView);
        result.ExplorerElementsView = newExplorerElementsView;
        await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }
    
    public async Task<CultureInfo> GetCurrentCultureAsync()
    {
        var result = (await serializer
                .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
            .CurrentCulture;
        logger.LogDebug(GetCurrentCultureLogMessage, result.ToString());
        return result;
    }

    public async Task<ExplorerElementsView> GetExplorerElementsViewAsync()
    {
        var result = (await serializer
                .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
            .ExplorerElementsView;
        logger.LogDebug(GetExplorerElementsViewLogMessage, result);
        return result;
    }
    
    public async Task<string> GetThemeNameAsync()
    {
       var result = (await serializer
           .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
           .ThemeName;
       logger.LogDebug(GetThemeNameLogMessage, result);
       return result;
    }
}