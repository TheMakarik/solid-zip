

namespace SolidZip.Services.CompressServices;

[ArchiveReader(".zip")]
public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger)
    : IArchiveReader
{
    private const string GetZipContentLogMessage = "Getting zip-archive content {path}, {archivePath}";
    
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

        logger.LogInformation(GetZipContentLogMessage, _path, directoryInArchive.Path);
        
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
            .Select(entry => new FileEntity(
                entry.FileName.ReplaceSeparatorsToDefault(), 
                IsDirectory: entry.IsDirectory, 
                IsArchiveEntry: true));
    }
    
    private IEnumerable<FileEntity> GetContent(FileEntity directoryInArchive)
    {
        var path = directoryInArchive.Path.ReplaceSeparatorsToAlt();
        return _zip.Entries
            .Where(entry => entry.FileName != path)
            .Where(entry => entry.FileName.StartsWith(path)) 
            .Select(entry => new FileEntity(
                entry.FileName.ReplaceSeparatorsToDefault(), 
                IsDirectory: entry.IsDirectory, 
                IsArchiveEntry: true));
    }
}