namespace SolidZip.Modules.Archiving;

public sealed class ArchiveSupportedExtensions(string[] extensions) : IArchiveSupportedExtensions
{
    public bool Contains(string extension)
    {
        return extensions.Contains(extension);
    }
}