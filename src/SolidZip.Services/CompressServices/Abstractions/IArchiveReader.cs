namespace SolidZip.Services.CompressServices.Abstractions;

public interface IArchiveReader : IDisposable
{
    public delegate string GetPasswordHandler(string path);
    public event GetPasswordHandler RequirePassword;
    public event GetPasswordHandler NotCorrectPassword;
    
    internal void SetPath(string path);
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive);

}