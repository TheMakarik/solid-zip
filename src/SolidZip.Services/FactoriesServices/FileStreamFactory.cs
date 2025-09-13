namespace SolidZip.Services.Factories;

internal sealed class FileStreamFactory : IFileStreamFactory
{
    public FileStream GetFactory(string path, FileMode mode)
    {
        return new FileStream(path, mode);
    }
}