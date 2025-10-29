namespace SolidZip.Model.Entities;

public class ArchiveConfiguration
{
    public int MaxPasswordRetries { get; set; }
    public bool SkipEncryptedEntries { get; set; }
}