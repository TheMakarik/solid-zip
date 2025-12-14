namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveDirectorySearcher
{
    public FileEntity Search(string path, string pattern, string archivePath, IArchiveReader reader);
}