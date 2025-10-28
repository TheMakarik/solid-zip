namespace SolidZip.Services.FactoriesServices;

public class ArchiveReaderFactory(ILogger<ArchiveReaderFactory> logger, IServiceProvider provider) : IArchiveReaderFactory
{
    public IArchiveReader? GetFactory(string archivePath)
    {
        var extension = Path.GetExtension(archivePath);
        return provider.GetKeyedService<IArchiveReader>(extension);
    }
}