using SolidZip.Model.EventArgs;

namespace SolidZip.Services.CompressServices;

[ArchiveReader(".zip")]
public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger, ArchiveConfiguration archiveConfiguration) : IArchiveReader
{
    private const string ReadArchiveLogMessage = "Successfully read zip archive {path}";
    private const string GetZipContentLogMessage = "Getting zip-archive content {path}, {archivePath}";
    private const string OpenEncryptedEntryLogMessage = "Successfully opened encrypted archive {Path}";

    private ZipFile _zip;
    private string _path = string.Empty;

    public event EventHandler<PasswordRequiredEventArgs> PasswordRequired;
    public event EventHandler<PasswordIncorrectEventArgs> PasswordIncorrect;

    public void SetPath(string path)
    {
        _path = path;
        InitializeZipFile();
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

    private void InitializeZipFile()
    {
        try
        {
            _zip = ZipFile.Read(_path);
            logger.LogDebug(ReadArchiveLogMessage, _path);
        }
        catch (BadPasswordException)
        {
            HandlePasswordRequired();
        }
    }

    private void HandlePasswordRequired()
    {
        var retryCount = 0;

        while (retryCount <= archiveConfiguration.MaxPasswordRetries)
        {
            var args = new PasswordRequiredEventArgs(_path);
            PasswordRequired?.Invoke(this, args);

            if (args.Cancel)
                throw new OperationCanceledException("Password input was cancelled");

            if (string.IsNullOrEmpty(args.Password))
                continue;

            try
            {
                _zip = ZipFile.Read(_path);
                _zip.Password = args.Password;
                
                logger.LogDebug(OpenEncryptedEntryLogMessage, _path);
                return; 
            }
            catch (BadPasswordException)
            {
                retryCount++;
                var incorrectArgs = new PasswordIncorrectEventArgs(_path);
                PasswordIncorrect?.Invoke(this, incorrectArgs);
            }
        }
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