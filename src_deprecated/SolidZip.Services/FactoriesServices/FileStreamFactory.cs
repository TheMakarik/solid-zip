namespace SolidZip.Services.FactoriesServices;

internal sealed class FileStreamFactory : IFileStreamFactory
{
    public FileStream GetFactory(string path, FileMode mode)
    {
        return new FileStream(path, mode);
    }
    
    public FileStream GetFactory(string path, FileMode mode, FileAccess access, FileShare share)
    {
        return new FileStream(path, mode, access, share);
    }
}