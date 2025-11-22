namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveReader : IDisposable
{
    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive);
    public void SetPath(string path);
}