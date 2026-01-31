using System.Windows.Forms;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Common.SevenZip;
using SharpCompress.Readers;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".7z")]
public class SevenZipArchiveReader(ILogger<SevenZipArchiveReader> logger,
    IMessageBox messageBox,
    IRequirePassword requirePassword) : IArchiveReader
{
    private SevenZipArchive _sevenZip;
    private string _path;

    [MemberNotNull(nameof(_sevenZip))]
    public void SetPath(string path)
    {
        _path = path;
       _sevenZip = SevenZipArchive.Open(path);
       if(IsEncrypted(_sevenZip))
           GetPassword();
      
    }

    private static bool IsRoot(string path)
    {
        return path == string.Empty ||
               path[0] == Path.DirectorySeparatorChar;
    }

    public Result<ExplorerResult, IEnumerable<FileEntity>> GetEntries(FileEntity directoryInArchive)
    {
        if (directoryInArchive.Path.StartsWith(_path) && !directoryInArchive.IsArchiveEntry)
            directoryInArchive = directoryInArchive with
            {
                IsArchiveEntry = true, Path = directoryInArchive.Path.CutPrefix(_path)
            };

        if (!directoryInArchive.IsArchiveEntry)
            throw new InvalidOperationException(
                $"Cannot get entries from {directoryInArchive.Path} in {_path} because it's not an archive entry");

        logger.LogInformation("Getting zip-archive content {path}, {archivePath}", _path, directoryInArchive.Path);

        var content = IsRoot(directoryInArchive.Path)
            ? GetRootContent()
            : throw new AxHost.InvalidActiveXStateException();

        return new Result<ExplorerResult, IEnumerable<FileEntity>>(ExplorerResult.Success, content.Select(e => e with {Path = Path.Combine(_path, e.Path)}));
    }
    
    private IEnumerable<FileEntity> GetRootContent()
    {
        var result =  _sevenZip.Entries
            .Where(entry =>
            {
                if(entry.Key is null)
                    return false;
                
                
                if (!entry.IsDirectory)
                    return !entry.Key.Contains(Path.AltDirectorySeparatorChar);

                var parts = entry.Key.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1 && entry.Key.EndsWith(Path.AltDirectorySeparatorChar);
            })
            .Select(CreateFileEntityFromSevenZipEntry).ToArray();
        return result;
    }
    
    private FileEntity CreateFileEntityFromSevenZipEntry(SevenZipEntry entry)
    {
        var path = entry.Key.ReplaceSeparatorsToDefault();

        return new FileEntity(
            path,
            entry.IsDirectory,
            entry.LastModifiedTime ?? DateTime.Now,
            entry.CreatedTime ?? DateTime.Now,
            entry.IsDirectory ? null : (ulong)entry.Size,
            null,
            true
        );
    }
    
    private static bool IsEncrypted(SevenZipArchive sevenZip)
    {
        return sevenZip.Entries.Any(entry => entry.IsEncrypted);
    }
    
    private void GetPassword()
    {
        _sevenZip.Dispose();
        var password = requirePassword.RequestPassword() ?? string.Empty;
        if(IsPasswordCorrect(_path, password))
            _sevenZip = SevenZipArchive.Open(_path, new ReaderOptions(){Password = password});
    }
    

    
    public void Dispose()
    {
       _sevenZip?.Dispose();
    }

    private static bool IsPasswordCorrect(string archiveFilePath, string password)
    {
        try
        {
            var options = new ReaderOptions { Password = password, LookForHeader = true };
            
            using var archive = SevenZipArchive.Open(archiveFilePath, options);
            foreach (var entry in archive.Entries)
            {
                if (entry.IsDirectory)
                    continue;

                using var stream = entry.OpenEntryStream();

                var buffer = new byte[100];
                stream.ReadExactly(buffer);

                return true;
            }

            return true;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}