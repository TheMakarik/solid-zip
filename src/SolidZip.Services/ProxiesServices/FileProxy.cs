namespace SolidZip.Services.Proxies;

internal sealed class FileProxy : IFileProxy
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }
}