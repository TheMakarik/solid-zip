namespace SolidZip.Services.ProxiesServices.Abstractions;

public interface IFileProxy
{
    public bool Exists(string path);
}