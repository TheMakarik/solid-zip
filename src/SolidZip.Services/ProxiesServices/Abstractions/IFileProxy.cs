namespace SolidZip.Services.Proxies.Abstractions;

public interface IFileProxy
{
    public bool Exists(string path);
}