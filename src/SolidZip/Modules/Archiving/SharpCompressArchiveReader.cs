using SharpCompress.Archives;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".tar", ".gz", ".gzip", ".rar", ".7z")]
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
            .Select(entry => new{Entry = entry,
                Path = entry.Key?.ReplaceSeparatorsToAlt()} //needs for .rar
            )
            .Where(entry => entry.Path?.TrimAlternativeDirectorySeparators() != pathToEntries)
            .Where(entry =>
            {
                Debug.Assert(entry.Path is not null);
                var searchParts = pathToEntries.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                var parts = entry.Path?.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries) ?? [];
                return (parts.Length == searchParts.Length + 1 
                        || (entry.Path!.EndsWith(Path.AltDirectorySeparatorChar) 
                            && parts.Length == searchParts.Length + 2))
                       && entry.Path!.StartsWith(pathToEntries);

            })
            .OrderBy(entry => entry.Entry.IsDirectory)
            .ThenBy(entry => entry.Path)
            .Select(e => ToFileEntity(e.Entry));
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, result.Select(e => e with {Path = Path.Combine(_path, e.Path)}));
    }

   

    private Result<ExplorerResult, IEnumerable<FileEntity>> GetRootContent()
    {
        var result = _archive.Entries
            .Where(e => e.Key is not null)
            .Where(entry =>
            {
                var key = entry.Key?.ReplaceSeparatorsToAlt(); //needs for .rar
                if (!entry.IsDirectory)
                    return !key!.Contains(Path.AltDirectorySeparatorChar);

                if (!key.Any(@char => @char == Path.AltDirectorySeparatorChar))
                    return true;
                
                var parts = key.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1 && key.EndsWith(Path.AltDirectorySeparatorChar);
            })
            .Select(ToFileEntity);
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, result.Select(e => e with {Path = Path.Combine(_path, e.Path)}));
    }


    
    private FileEntity ToFileEntity(IArchiveEntry archiveEntry)
    {
        return new FileEntity()
        {
            Path = archiveEntry.Key!.ReplaceSeparatorsToAlt().CutPrefix(_path).ReplaceSeparatorsToDefault(),
            IsArchiveEntry = true,
            IsDirectory = archiveEntry.IsDirectory,
            Comment = string.Empty,
            BytesSize = (ulong?)archiveEntry.CompressedSize,
            CreationalTime = archiveEntry.CreatedTime.GetValueOrDefault(),
            LastChanging = archiveEntry.LastModifiedTime.GetValueOrDefault()
        };
    }
}