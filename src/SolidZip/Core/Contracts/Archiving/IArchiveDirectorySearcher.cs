namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveDirectorySearcher
{
    public string Search(string path, string pattern, IArchiveReader reader);
}