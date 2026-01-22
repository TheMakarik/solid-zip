namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".zip")]
public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger)
    : IArchiveReader
{
    private ZipFile _zip;
    private string _path = string.Empty;
    
    public void SetPath(string path)
    {
        _path = path;
        _zip = ZipFile.Read(_path,  new(){Encoding = Encoding.UTF8});
    }
    
    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive)
    {
        if(directoryInArchive.Path.StartsWith(_path) && !directoryInArchive.IsArchiveEntry)
            directoryInArchive = directoryInArchive with { IsArchiveEntry = true,  Path =  directoryInArchive.Path.CutPrefix(_path)};
        
        if (!directoryInArchive.IsArchiveEntry)
            throw new InvalidOperationException($"Cannot get entries from {directoryInArchive.Path} in {_path} because it's not an archive entry");

        logger.LogInformation("Getting zip-archive content {path}, {archivePath}", _path, directoryInArchive.Path);

        var content =  IsRoot(directoryInArchive.Path)
            ? GetRootContent() 
            : GetContent(directoryInArchive.Path);
        
        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, content);
      
    }

    public void Dispose()
    {
        _zip?.Dispose();
    }
    
    private static bool IsRoot(string path)
    {
        return path == string.Empty ||
               path[0] == Path.DirectorySeparatorChar;
    }
    
    private IEnumerable<FileEntity> GetContent(string path)
    {
        path = path.ReplaceSeparatorsToAlt();
    
        return _zip.Entries
            .Where(entry => entry.FileName != path)
            .Where(entry => entry.FileName.StartsWith(path))
            .Where(entry =>
            {
              
                if (entry.IsDirectory)
                {
                    var relativePath = entry.FileName.Substring(path.Length);
                    var parts = relativePath.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length == 1; 
                }
                else
                {
                    var relativePath = entry.FileName.Substring(path.Length);
                    var parts = relativePath.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length == 1 && !relativePath.EndsWith(Path.AltDirectorySeparatorChar);
                }
            })
            .Select(CreateFileEntityFromZipEntry);
    }

    private IEnumerable<FileEntity> GetRootContent()
    {
        return _zip.Entries
            .Where(entry => 
            {
                
                if (!entry.IsDirectory)
                    return !entry.FileName.Contains(Path.AltDirectorySeparatorChar);
                
                var parts = entry.FileName.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1 && entry.FileName.EndsWith(Path.AltDirectorySeparatorChar);

            })
            .Select(CreateFileEntityFromZipEntry);
    }

    private FileEntity CreateFileEntityFromZipEntry(ZipEntry entry)
    {
        var path = entry.FileName.ReplaceSeparatorsToDefault();
        
        return new FileEntity(
            Path: path,
            IsDirectory: entry.IsDirectory,
            LastChanging: entry.ModifiedTime,
            CreationalTime: entry.CreationTime,
            BytesSize: entry.IsDirectory ? null : (ulong)entry.UncompressedSize,
            Comment: entry.Comment,
            IsArchiveEntry: true
        );
    }
}