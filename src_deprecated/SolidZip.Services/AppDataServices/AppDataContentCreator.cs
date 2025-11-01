
namespace SolidZip.Services.AppDataServices;

internal sealed class AppDataContentCreator(
    ILogger<AppDataContentCreator> logger,
    IFileProxy fileProxy,
    IOptions<AppDataOptions> appDataOptions,
    IFileStreamFactory streamFactory,
    IJsonSerializer serializer
    ) : IAppDataContentCreator
{
    private const string AppDataFileExistsLogMessage = "{path} is already exists";
    private const string CreatingAppDataFile = "Creating app data file: {path}";
    
    private readonly string _appDataUserFilePath = Environment.ExpandEnvironmentVariables(appDataOptions.Value.DataJsonFilePath);
    
    public async ValueTask CreateAsync()
    {
        if (AppDataUserFileExists())
            logger.LogInformation(AppDataFileExistsLogMessage, _appDataUserFilePath);
        else
            await CreateAppDataUserFileAsync();
    }
    

    private bool AppDataUserFileExists()
    {
        return fileProxy.Exists(_appDataUserFilePath);
    }
    
    private async Task CreateAppDataUserFileAsync()
    {
        logger.LogInformation(CreatingAppDataFile, _appDataUserFilePath);
        var appDataFileContent = appDataOptions.Value.Defaults;
        await using var stream = streamFactory.GetFactory(_appDataUserFilePath, FileMode.Create);
        await serializer.SerializeAsync(appDataFileContent, stream, _appDataUserFilePath, AppDataContentSerializerContext.Default.Options);
    }
}