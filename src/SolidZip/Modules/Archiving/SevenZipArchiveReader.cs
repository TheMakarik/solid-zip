using SharpCompress.Archives.SevenZip;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".7z")]
public class SevenZipArchiveReader : IArchiveReader
{
    private SevenZipArchive _sevenZip;

    public void SetPath(string path)
    {
       _sevenZip = SevenZipArchive.Open(path);
    }

    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive)
    {
        throw new NotImplementedException();
    }
    
    
    public void Dispose()
    {
       _sevenZip.Dispose();
    }
}