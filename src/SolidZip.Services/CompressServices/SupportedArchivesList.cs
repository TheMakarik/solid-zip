namespace SolidZip.Services.CompressServices;

public class SupportedArchivesList : ISupportedArchivesList
{
    private readonly ConcurrentBag<string> _supportedExtensions = new();
    
    public void AddArchiveExtension(string extension)
    {
        _supportedExtensions.Add(extension);
    }
    
    public bool IsSupported(string extension)
    {
        return _supportedExtensions.Contains(extension);
    }
}