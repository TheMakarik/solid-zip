namespace SolidZip.Services.Proxies.Abstractions;

public interface IDirectoryProxy
{
    public  string[] GetLogicalDrives();
    public IEnumerable<string> EnumerateFiles(string path);
    public IEnumerable<string> EnumerableDirectories(string path);
    public bool Exists(string path);
}