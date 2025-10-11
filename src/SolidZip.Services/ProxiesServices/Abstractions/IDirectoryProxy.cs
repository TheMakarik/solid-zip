namespace SolidZip.Services.ProxiesServices.Abstractions;

public interface IDirectoryProxy
{
    public  string[] GetLogicalDrives();
    public IEnumerable<string> EnumerateFiles(string path);
    public IEnumerable<string> EnumerateDirectories(string path);
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption options);
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption options);
    public bool Exists(string path);
}