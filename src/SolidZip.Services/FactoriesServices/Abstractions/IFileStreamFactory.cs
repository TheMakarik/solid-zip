namespace SolidZip.Services.Factories.Abstractions;

public interface IFileStreamFactory
{
    public FileStream GetFactory(string path, FileMode mode);
}