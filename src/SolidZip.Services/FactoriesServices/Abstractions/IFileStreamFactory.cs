namespace SolidZip.Services.FactoriesServices.Abstractions;

public interface IFileStreamFactory
{
    public FileStream GetFactory(string path, FileMode mode);
    public FileStream GetFactory(string path, FileMode mode, FileAccess access, FileShare share);
}