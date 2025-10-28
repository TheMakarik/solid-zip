namespace SolidZip.Services.CompressServices;

[ArchiveReader(".zip")]
public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger) : IArchiveReader
{
    private const string ReadArchiveLogMessage = "Successefully read zip archive {path}";
    private const string GetZipContentLogMessage = "Gettting zip-archive content {path}, {archivePath}";
    
    private ZipFile _zip;
    private string _path = string.Empty;

    public event IArchiveReader.GetPasswordHandler? RequirePassword;
    public event IArchiveReader.GetPasswordHandler? NotCorrectPassword;

    public void SetPath(string path)
    {
        _path = path;
       
        _zip = ZipFile.Read(path);
        logger.LogDebug(ReadArchiveLogMessage, path);
    }
    
    public IEnumerable<FileEntity> GetEntries(FileEntity directoryInArchive)
    {
        ExceptionHelper.ThrowIf(!directoryInArchive.IsArchiveEntry, () => new InvalidOperationException($"Cannot get entries from {directoryInArchive.Path} in {_path} because this it's not an archive entry") );

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
        if (_zip.Encryption != EncryptionAlgorithm.None || _zip.Encryption != EncryptionAlgorithm.Unsupported)
            RequirePassword?.Invoke(_path);
        
        return _zip.Entries
            .Where(entry => !entry.FileName.Contains(Path.AltDirectorySeparatorChar))//not in folder
            .Select(entry => new FileEntity(entry.FileName.ReplaceSeparatorsToDefault(), IsDirectory: entry.IsDirectory, IsArchiveEntry: true));
         
    }
    
    private IEnumerable<FileEntity> GetContent(FileEntity directoryInArchive)
    {
        var path = directoryInArchive.Path.ReplaceSeparatorsToAlt();
        return _zip.Entries
            .Where(entry => entry.FileName != path)
            .Where(entry => entry.FileName.StartsWith(path)) 
            .Select(entry => new FileEntity(entry.FileName.ReplaceSeparatorsToDefault(), IsDirectory: entry.IsDirectory, IsArchiveEntry: true));
    }
}
