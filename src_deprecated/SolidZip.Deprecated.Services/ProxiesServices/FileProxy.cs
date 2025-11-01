using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.ProxiesServices;

internal sealed class FileProxy : IFileProxy
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }
}