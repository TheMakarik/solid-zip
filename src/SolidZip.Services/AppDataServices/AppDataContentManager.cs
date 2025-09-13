namespace SolidZip.Services.AppDataServices;

public class AppDataContentManager(IJsonSerializer serializer, IOptions<AppDataOptions> options) : IAppDataContentManager
{
    private readonly string _appDataFilePath = Environment.ExpandEnvironmentVariables(options.Value.DataJsonFilePath);
    
    public async Task ChangeThemeNameAsync(string newThemeName)
    {
       var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
       result.ThemeName = newThemeName;
       await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task ChangeCurrentCultureAsync(CultureInfo newCurrentCulture)
    {
        var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
        result.CurrentCulture = newCurrentCulture;
        await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task ChangeExplorerElementsView(ExplorerElementsView newExplorerElementsView)
    {
        var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
        result.ExplorerElementsView = newExplorerElementsView;
        await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task ChangeUseCustomIconsCollectionAsync(bool newUseCustomIconsCollection)
    {
        var result = await serializer.DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options);
        result.UseCustomIconsCollection = newUseCustomIconsCollection;
        await serializer.SerializeAsync(result, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }

    public async Task<CultureInfo> GetCurrentCultureAsync()
    {
        return (await serializer
                .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
            .CurrentCulture;
    }

    public async Task<ExplorerElementsView> GetExplorerElementsViewAsync()
    {
        return (await serializer
                .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
            .ExplorerElementsView;
    }

    public async Task<bool> GetUseCustomIconsCollectionAsync()
    {
        return (await serializer
                .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
            .UseCustomIconsCollection;
    }

    public async Task<string> GetThemeNameAsync()
    {
       return (await serializer
           .DeserializeAsync<AppDataContent>(_appDataFilePath, AppDataContentSerializerContext.Default.Options))
           .ThemeName;
    }
}