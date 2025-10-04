using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.ProxiesServices;

internal sealed class DirectoryProxy : IDirectoryProxy
{
    public string[] GetLogicalDrives()
    {
        return Directory.GetLogicalDrives();
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        return Directory.EnumerateFiles(path);
    }

    public IEnumerable<string> EnumerableDirectories(string path)
    {
        return Directory.EnumerateDirectories(path);
    }

    public bool Exists(string path)
    {
        return Directory.Exists(path);
    }
}