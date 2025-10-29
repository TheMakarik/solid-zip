using SolidZip.Model.EventArgs;

namespace SolidZip.Services.CompressServices.Abstractions;

public interface IArchiveReader : IDisposable
{
    event EventHandler<PasswordRequiredEventArgs> PasswordRequired;
    event EventHandler<PasswordIncorrectEventArgs> PasswordIncorrect;
    
    internal void SetPath(string path);
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive);

}