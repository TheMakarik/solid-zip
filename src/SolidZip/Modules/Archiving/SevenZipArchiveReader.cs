using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".7z")]
public class SevenZipArchiveReader(ILogger<SevenZipArchiveReader> logger, IMessageBox messageBox, IRequirePassword requirePassword, IEncodingDetector encodingDetector) : IArchiveReader
{
    private SevenZipArchive _sevenZip;

    public void SetPath(string path)
    {
       _sevenZip = SevenZipArchive.Open(path, new ReaderOptions(){});
      
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