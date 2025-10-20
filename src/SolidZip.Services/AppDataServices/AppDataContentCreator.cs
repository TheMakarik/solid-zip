
namespace SolidZip.Services.AppDataServices;

internal sealed class AppDataContentCreator(
    ILogger<AppDataContentCreator> logger,
    IFileProxy fileProxy,
    IOptions<AppDataOptions> options,
    IFileStreamFactory streamFactory,
    IJsonSerializer serializer
    ) : IAppDataContentCreator
{
    private const string AppDataFileExistsLogMessage = "{path} is already exists";
    private const string CreatingAppDataFile = "Creating app data file: {path}";
    private readonly string _appDataFilePath = Environment.ExpandEnvironmentVariables(options.Value.DataJsonFilePath);
    
    public async ValueTask CreateAsync()
    {
        if (AppDataFileExists())
            logger.LogInformation(AppDataFileExistsLogMessage, _appDataFilePath);
        else
            await CreateAppDataFileAsync();
    }

    private bool AppDataFileExists()
    {
        return fileProxy.Exists(_appDataFilePath);
    }

    private async Task CreateAppDataFileAsync()
    {
        logger.LogInformation(CreatingAppDataFile, _appDataFilePath);
        var appDataFileContent = options.Value.Defaults;
        await using var stream = streamFactory.GetFactory(_appDataFilePath, FileMode.Create);
        await serializer.SerializeAsync(appDataFileContent, stream, _appDataFilePath, AppDataContentSerializerContext.Default.Options);
    }
}