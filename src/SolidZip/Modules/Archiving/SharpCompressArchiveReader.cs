using SharpCompress.Archives;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(
    ".tar", ".zst", ".tgz", ".arc", ".arj", ".gz",
    ".gzip", ".zstd", ".7z", ".rar", 
    ".lz", ".lzip", ".xz", ".bz2")]
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
        ArchiveReaderHelper.PrepareFileEntity(ref directoryInArchive, _path);
        
        logger.LogInformation("Getting archive content {path}, {archivePath}", _path, directoryInArchive.Path);

        return ArchiveReaderHelper.IsRoot(directoryInArchive.Path, _path)
            ? GetRootContent()
            : GetContent(directoryInArchive.Path);
    }

    public void Dispose()
    {
        _archive.Dispose();
    }
    
    
    private Result<ExplorerResult, IEnumerable<FileEntity>> GetContent(string directory)
    {
        var searchDirectory = _archive.Entries.FirstOrDefault(e => e.Key == directory);
        
        if (searchDirectory is not null && !searchDirectory.IsDirectory)
            return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.NotDirectory, []);
            
        var pathToEntries = directory.CutPrefix(_path).ReplaceSeparatorsToAlt();
        pathToEntries = pathToEntries.TrimAlternativeDirectorySeparators();
        var result = _archive.Entries
            .Where(entry => entry.Key is not null)
            .Where(entry => entry.Key!.TrimAlternativeDirectorySeparators() != pathToEntries)
            .Where(entry => entry.Key!.StartsWith(pathToEntries))
            .OrderBy(entry => !entry.IsDirectory)
            .ThenBy(entry => entry.Key)
            .Select(ToFileEntity);
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, result.Select(e => e with {Path = Path.Combine(_path, e.Path)}));
    }

   

    private Result<ExplorerResult, IEnumerable<FileEntity>> GetRootContent()
    {
        var result = _archive.Entries
            .Where(e => e.Key is not null)
            .Where(entry =>
            {
                if (!entry.IsDirectory)
                    return !entry.Key!.Contains(Path.AltDirectorySeparatorChar);

                if (!entry.Key!.Any(@char => @char == Path.AltDirectorySeparatorChar))
                    return true;
                
                var parts = entry.Key!.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1 && entry.Key!.EndsWith(Path.AltDirectorySeparatorChar);
            })
            .Select(ToFileEntity);
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, result.Select(e => e with {Path = Path.Combine(_path, e.Path)}));
    }


    
    private FileEntity ToFileEntity(IArchiveEntry archiveEntry)
    {
        return new FileEntity()
        {
            Path = archiveEntry.Key!.CutPrefix(_path).ReplaceSeparatorsToDefault(),
            IsArchiveEntry = true,
            IsDirectory = archiveEntry.IsDirectory,
            Comment = null,
            BytesSize = (ulong?)archiveEntry.CompressedSize,
            CreationalTime = archiveEntry.CreatedTime.GetValueOrDefault(),
            LastChanging = archiveEntry.LastModifiedTime.GetValueOrDefault()
        };
    }
}