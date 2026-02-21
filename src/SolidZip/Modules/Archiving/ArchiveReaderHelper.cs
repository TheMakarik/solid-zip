namespace SolidZip.Modules.Archiving;

public static class ArchiveReaderHelper
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

    public static bool IsRoot(string path,  string absoluteArchivePath)
    {
        var pathToCheck = path.CutPrefix(absoluteArchivePath); 
        return pathToCheck == string.Empty || pathToCheck.Length == 1;
    }
}