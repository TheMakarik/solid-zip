namespace SolidZip.Core.Contracts.Archiving;

public interface IArchiveReader : IDisposable
{
    public IEnumerable<FileEntity> Entries { get; }
    public Task ExtractAll(string toDirectory, IProgress<double> progress, ArchiveExtractingOptions options, CancellationToken cancel);
    public Task ExtractOnly(string name, string toDirectory, IProgress<double> progress, ArchiveExtractingOptions options, CancellationToken cancel);
    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive);
    public void SetPath(string path);
 
}