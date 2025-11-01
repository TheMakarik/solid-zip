namespace SolidZip.Services.CompressServices.Abstractions;

public interface IArchiveReader : IDisposable
{
    internal void SetPath(string path);
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive);

}