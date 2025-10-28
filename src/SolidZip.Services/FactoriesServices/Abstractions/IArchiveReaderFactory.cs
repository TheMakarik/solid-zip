namespace SolidZip.Services.FactoriesServices.Abstractions;

public interface IArchiveReaderFactory
{
    public IArchiveReader? GetFactory(string archivePath);
}