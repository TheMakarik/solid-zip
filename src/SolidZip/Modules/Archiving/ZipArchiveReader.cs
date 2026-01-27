using ZipFile = Ionic.Zip.ZipFile;

namespace SolidZip.Modules.Archiving;

[ArchiveExtensions(".zip")]
public sealed class ZipArchiveReader(ILogger<ZipArchiveReader> logger, IEncodingDetector encodingDetector, IOptions<EncodingOptions> encodingOptions)
    : IArchiveReader
{
    private string _path = string.Empty;
    private ZipFile _zip;

    public void SetPath(string path)
    {
        var encoding = DetectEncoding(path);
        _path = path;
        _zip = ZipFile.Read(_path,  new ReadOptions { Encoding = encoding});
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
    
    private Encoding DetectEncoding(string path)
    {
        logger.LogInformation("Detecting zip-archive encoding {path}", _path);
        foreach (var encodingString in encodingOptions.Value.EncodingBeforeAutodetect)
        {
            var encoding = Encoding.GetEncoding(encodingString);
            using var zip = ZipFile.Read(path, new ReadOptions{ Encoding =  encoding });
            var entries = zip.Entries.Select(e => e.FileName);
            
            if (HasBrokenEncoding(entries)) 
                continue;
            
            logger.LogInformation("Found encoding {encoding}", encodingString);
            return encoding;
        }
        return encodingDetector.DetectEncoding(path);
    }

    private bool HasBrokenEncoding(IEnumerable<string> entries)
    {
        var errorChars = encodingOptions.Value.EncodingErrorsChars;
        return (from entry in entries from errorChar in errorChars where entry.Contains(errorChar) select entry).Any();
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
                    var parts = relativePath.Split(Path.AltDirectorySeparatorChar,
                        StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length == 1;
                }
                else
                {
                    var relativePath = entry.FileName.Substring(path.Length);
                    var parts = relativePath.Split(Path.AltDirectorySeparatorChar,
                        StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length == 1 && !relativePath.EndsWith(Path.AltDirectorySeparatorChar);
                }
            })
            .Select(CreateFileEntityFromZipEntry);
    }

    private IEnumerable<FileEntity> GetRootContent()
    {
        var result =  _zip.Entries
            .Where(entry =>
            {
                if (!entry.IsDirectory)
                    return !entry.FileName.Contains(Path.AltDirectorySeparatorChar);

                var parts = entry.FileName.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length == 1 && entry.FileName.EndsWith(Path.AltDirectorySeparatorChar);
            })
            .Select(CreateFileEntityFromZipEntry).ToArray();
        return result;
    }

    private FileEntity CreateFileEntityFromZipEntry(ZipEntry entry)
    {
        var path = entry.FileName.ReplaceSeparatorsToDefault();

        return new FileEntity(
            path,
            entry.IsDirectory,
            entry.ModifiedTime,
            entry.CreationTime,
            entry.IsDirectory ? null : (ulong)entry.UncompressedSize,
            entry.Comment,
            true
        );
    }
}