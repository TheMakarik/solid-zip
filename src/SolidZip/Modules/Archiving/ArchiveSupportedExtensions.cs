namespace SolidZip.Modules.Archiving;

public class ArchiveSupportedExtensions(string[] extensions) : IArchiveSupportedExtensions
{
    public bool Contains(string extension)
    {
        return extensions.Contains(extension);
    }
}