namespace SolidZip.Modules.AppData;

public class UserJsonCreator(
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
        cache.Value = defaults.Value.UserData; //optimization trick 
        await JsonSerializer.SerializeAsync(stream, defaults.Value.UserData,  UserDataSerializerContext.Default.Options);
        logger.LogInformation("{data} was created with value: {value}", pathsCollection.UserData,  defaults.Value.UserData);
    }
}