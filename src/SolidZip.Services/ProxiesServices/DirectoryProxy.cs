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

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        return Directory.EnumerateDirectories(path);
    }
    
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption options)
    {
        return Directory.EnumerateFiles(path, searchPattern, options);
    }

    public bool Exists(string path)
    {
        return Directory.Exists(path);
    }
}