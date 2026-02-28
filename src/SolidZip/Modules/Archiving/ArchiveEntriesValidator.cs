namespace SolidZip.Modules.Archiving;

public class ArchiveEntriesValidator : IArchiveEntriesValidator
{
    public bool IsSubEntryOf(string entry, string subEntry)
    {
        var searchParts = entry.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var parts = subEntry.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries) ?? [];
        return (parts.Length == searchParts.Length + 1 
                || (subEntry.EndsWith(Path.AltDirectorySeparatorChar) 
                    && parts.Length == searchParts.Length + 2))
               && subEntry.StartsWith(entry);
    }
}