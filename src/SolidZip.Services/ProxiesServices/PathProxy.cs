using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.ProxiesServices;

public class PathProxy : IPathProxy
{
    public string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }
}