
namespace SolidZip.Services.AppDataServices;

internal sealed class AppDataContentCreator(
    ILogger<AppDataContentCreator> logger,
    IFileProxy fileProxy,
    IOptions<AppDataOptions> appDataOptions,
    IFileStreamFactory streamFactory,
    IOptions<ArchiveOptions> archiveOptions,
    IJsonSerializer serializer
    ) : IAppDataContentCreator
{
    private const string AppDataFileExistsLogMessage = "{path} is already exists";
    private const string CreatingAppDataFile = "Creating app data file: {path}";
    
    private readonly string _appDataUserFilePath = Environment.ExpandEnvironmentVariables(appDataOptions.Value.DataJsonFilePath);
    private readonly string _archiveFilePath = Environment.ExpandEnvironmentVariables(archiveOptions.Value.ArchiveConfigurationPath);
    
    public async ValueTask CreateAsync()
    {
        await CreateUserJsonFile();
        await CreateArchiveFile();
    }

    private async ValueTask CreateArchiveFile()
    {
        if (AppDataArchiveFileExists())
            logger.LogInformation(AppDataFileExistsLogMessage, _archiveFilePath);
        else
            await CreateAppDataArchiveFileAsync();

    }
    
    private async ValueTask CreateUserJsonFile()
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
    
    private bool AppDataArchiveFileExists()
    {
        return fileProxy.Exists(_archiveFilePath);
    }
    
    private async Task CreateAppDataArchiveFileAsync()
    {
        logger.LogInformation(CreatingAppDataFile, _archiveFilePath);
        var configuration = archiveOptions.Value.DefaultConfiguration;
        await using var stream = streamFactory.GetFactory(_archiveFilePath, FileMode.Create);
        await serializer.SerializeAsync(configuration, stream, _archiveFilePath, ArchiveSerializerContext.Default.Options);
    }


    private async Task CreateAppDataUserFileAsync()
    {
        logger.LogInformation(CreatingAppDataFile, _appDataUserFilePath);
        var appDataFileContent = appDataOptions.Value.Defaults;
        await using var stream = streamFactory.GetFactory(_appDataUserFilePath, FileMode.Create);
        await serializer.SerializeAsync(appDataFileContent, stream, _appDataUserFilePath, AppDataContentSerializerContext.Default.Options);
    }
}