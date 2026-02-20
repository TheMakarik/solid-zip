using SharpCompress.Archives;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(
    ".tar", 
    ".zst",
    ".tgz",
    ".arc",
    ".arj",
    ".gz",
    ".gzip",
    ".zstd",
    ".7z",
    ".rar", 
    ".lz",
    ".lzip",
    ".xz",
    ".bz2")]
public sealed class SharpCompressArchiveReader : IArchiveReader
{
    private string _path = string.Empty;
    private IArchive _archive;
    
    public void SetPath(string path)
    {
        _path = path;
        _archive = ArchiveFactory.Open(_path);
    }


    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive)
    {
        throw new NotImplementedException();
    }

  
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}