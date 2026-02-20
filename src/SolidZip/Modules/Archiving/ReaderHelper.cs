namespace SolidZip.Modules.Archiving;

public static class ReaderHelper
{
    public static void PrepareFileEntity(ref FileEntity fileEntity, string archivePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(archivePath);
        
        if (fileEntity.Path.StartsWith(archivePath) && !fileEntity.IsArchiveEntry)
            fileEntity = fileEntity with
            {
                IsArchiveEntry = true, Path = fileEntity.Path.CutPrefix(archivePath)
            };

        if (!fileEntity.IsArchiveEntry)
            throw new InvalidOperationException(
                $"Cannot get entries from {fileEntity.Path} in {archivePath} because it's not an archive entry");


    }
}