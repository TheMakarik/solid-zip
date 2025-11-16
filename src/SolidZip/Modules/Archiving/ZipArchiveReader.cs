namespace SolidZip.Modules.Archiving;

public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger)
    : IArchiveReader
{
    private ZipFile _zip;
    private string _path = string.Empty;
    
    public void SetPath(string path)
    {
        _path = path;
    }
    
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive)
    {
        if (!directoryInArchive.IsArchiveEntry)
            throw new InvalidOperationException($"Cannot get entries from {directoryInArchive.Path} in {_path} because it's not an archive entry");

        logger.LogInformation("Getting zip-archive content {path}, {archivePath}", _path, directoryInArchive.Path);
        
        return IsRoot(directoryInArchive)
            ? GetRootContent(directoryInArchive) 
            : GetContent(directoryInArchive);
    }

    public void Dispose()
    {
        _zip?.Dispose();
    }
    
    private static bool IsRoot(FileEntity directoryInArchive)
    {
        return directoryInArchive.Path == string.Empty ||
               directoryInArchive.Path == Path.DirectorySeparatorChar.ToString();
    }
    
    private IEnumerable<FileEntity> GetRootContent(FileEntity directoryInArchive)
    {
        return _zip.Entries
            .Where(entry => !entry.FileName.Contains(Path.AltDirectorySeparatorChar))
            .Select(entry => CreateFileEntityFromZipEntry(entry));
    }
    
    private IEnumerable<FileEntity> GetContent(FileEntity directoryInArchive)
    {
        var path = directoryInArchive.Path.ReplaceSeparatorsToAlt();
        return _zip.Entries
            .Where(entry => entry.FileName != path)
            .Where(entry => entry.FileName.StartsWith(path)) 
            .Select(entry => CreateFileEntityFromZipEntry(entry));
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