namespace SolidZip.Modules.AppData;

public sealed class UserJsonCreator(
    IOptions<LocalizationOptions> localizationOptions,
    ILogger<UserJsonCreator> logger,
    SharedCache<UserData> cache,
    IOptions<DefaultOptions> defaults,
    PathsCollection pathsCollection) : IUserJsonCreator
{
    public async ValueTask CreateAsync()
    {
        if (File.Exists(pathsCollection.UserData))
        {
            logger.LogInformation("{data} file is already exists", pathsCollection.UserData);
            return;
        }

        await CreateUserDataAsync();
    }

    private async Task CreateUserDataAsync()
    {
        await using var stream = new FileStream(pathsCollection.UserData, FileMode.Create);

        var defaultUserData = LoadUserDataWithLocalization();

        cache.Value = defaultUserData; //Optimization trick
        await JsonSerializer.SerializeAsync(stream, defaultUserData, UserDataSerializerContext.Default.Options);
        logger.LogInformation("{data} was created with value: {value}", pathsCollection.UserData,
            defaults.Value.UserData);
    }

    private UserData LoadUserDataWithLocalization()
    {
        var defaultUserData = defaults.Value.UserData;
        if (defaultUserData.CurrentCulture.ToString() == string.Empty)
            defaultUserData.CurrentCulture =
                localizationOptions.Value.SupportedCultures.Values.Contains(CultureInfo.CurrentUICulture)
                    ? CultureInfo.CurrentUICulture
                    : new CultureInfo("en-US");
        logger.LogInformation("Localization was loaded: {localization}", defaultUserData.CurrentCulture);
        return defaultUserData;
    }
}