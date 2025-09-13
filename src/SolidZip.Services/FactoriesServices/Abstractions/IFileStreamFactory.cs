namespace SolidZip.Services.FactoriesServices.Abstractions;

public interface IFileStreamFactory
{
    public FileStream GetFactory(string path, FileMode mode);
}