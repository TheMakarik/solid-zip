namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveEntriesValidator
{
    public bool IsSubEntryOf(string entry, string subEntry);
}