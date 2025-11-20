namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveReader : IDisposable
{
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive);
    public void SetPath(string path);
}