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
public sealed class SharpCompressArchiveReader(ILogger<ZipArchiveReader> logger, 
    IRequirePassword requirePassword,
    IMessageBox messageBox,
    IEncodingDetector encodingDetector, 
    IOptions<EncodingOptions> encodingOptions) : IArchiveReader
{
    private string _path = string.Empty;
    private IArchive _archive;
    
    public void SetPath(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        _path = path;
        _archive = ArchiveFactory.Open(_path);
    }


    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive)
    {
        ReaderHelper.PrepareFileEntity(ref directoryInArchive, _path);
        
        logger.LogInformation("Getting archive content {path}, {archivePath}", _path, directoryInArchive.Path);
        
    }

  
    public void Dispose()
    {
        _archive.Dispose();
    }
}