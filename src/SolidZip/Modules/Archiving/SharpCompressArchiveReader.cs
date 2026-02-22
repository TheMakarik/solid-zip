using SharpCompress.Archives;
using SharpCompress.Common;

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

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public IEnumerable<FileEntity> Entries => _archive is null ? [] : _archive.Entries.Select(ToFileEntity);
    
    public void SetPath(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        _path = path;
        _archive = ArchiveFactory.Open(_path);
    }


  
    public async Task ExtractAll(string toDirectory, IProgress<double> progress, ArchiveExtractingOptions options, CancellationToken cancel)
    {
        var sharpCompressStubProgress = new Progress<ProgressReport>();
        var alreadyCompressedContent = new string[_archive.Entries.Count()];
        var alreadyCompressedContentCapacity = 0;
        sharpCompressStubProgress.ProgressChanged += (_, args) =>
        {
            if (args.PercentComplete is null)
                logger.LogError("Per cent complete is null then extracting {path} to {dir} ", _path, toDirectory);
            progress.Report(args.PercentComplete.GetValueOrDefault());
            logger.LogDebug("Extracted: {name}", args.EntryPath);
            
            alreadyCompressedContent[alreadyCompressedContentCapacity] =  args.EntryPath;
            alreadyCompressedContentCapacity++;
        };
        
        try
        {
            logger.LogInformation("Start extracting {path} to {to}", _path, toDirectory);
            await _archive.WriteToDirectoryAsync(toDirectory, new ExtractionOptions()
            {
            }, sharpCompressStubProgress, cancel);
        }
        catch (OperationCanceledException e)
        {
            logger.LogInformation("Extracting {path} to {to} cancelled", toDirectory, toDirectory);
        }
     
    }

    public Task ExtractOnly(string name, string toDirectory, IProgress<double> progress,  ArchiveExtractingOptions options, CancellationToken cancel)
    {
        throw new NotImplementedException();
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
            .Select(entry => new{SharpCompressEntry = entry,
                FileName = entry.Key?.ReplaceSeparatorsToAlt()} //needs for .rar
            )
            .Where(entry => entry.FileName?
                .TrimAlternativeDirectorySeparators() != pathToEntries)
            .Where(entry =>
            {
                Debug.Assert(entry.FileName is not null);
                var searchParts = pathToEntries.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                var parts = entry.FileName?.Split(Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries) ?? [];
                return (parts.Length == searchParts.Length + 1 
                        || (entry.FileName!.EndsWith(Path.AltDirectorySeparatorChar) 
                            && parts.Length == searchParts.Length + 2))
                       && entry.FileName!.StartsWith(pathToEntries);

            })
            .OrderBy(entry => entry.SharpCompressEntry.IsDirectory)
            .ThenBy(entry => entry.FileName)
            .Select(e => ToFileEntity(e.SharpCompressEntry));
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
