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
        _zip = ZipFile.Read(_path);
    }
    
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive)
    {
        if (!directoryInArchive.IsArchiveEntry)
            throw new InvalidOperationException($"Cannot get entries from {directoryInArchive.Path} in {_path} because it's not an archive entry");

        logger.LogInformation("Getting zip-archive content {path}, {archivePath}", _path, directoryInArchive.Path);
        
        return IsRoot(directoryInArchive.Path)
            ? GetRootContent() 
            : GetContent(directoryInArchive.Path);
    }

    public void Dispose()
    {
        _zip?.Dispose();
    }
    
    private static bool IsRoot(string path)
    {
        return path == string.Empty ||
               path == Path.DirectorySeparatorChar.ToString();
    }
    
    private IEnumerable<FileEntity> GetRootContent()
    {
        return _zip.Entries
            .Where(entry => !entry.FileName.Contains(Path.AltDirectorySeparatorChar))
            .Select(entry => CreateFileEntityFromZipEntry(entry));
    }
    
    private IEnumerable<FileEntity> GetContent(string path)
    {
        path = path.ReplaceSeparatorsToAlt();
        return _zip.Entries
            .Where(entry => entry.FileName != path)
            .Where(entry => entry.FileName.StartsWith(path)) 
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