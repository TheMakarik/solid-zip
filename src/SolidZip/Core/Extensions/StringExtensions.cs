namespace SolidZip.Core.Extensions;

public static class StringExtensions
{
    public static FileEntity ToDirectoryFileEntity(this string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        return new FileEntity(
            path,
            IsDirectory: true,
            directoryInfo.LastWriteTime,
            directoryInfo.CreationTime, null);
    }

    public static FileEntity ToFileEntity(this string path)
    {
        var fileInfo = new FileInfo(path);
        return new FileEntity(path,
            IsDirectory: false,
            fileInfo.LastWriteTime,
            fileInfo.CreationTime,
            (ulong)fileInfo.Length);
    }
    
    public static string ReplaceSeparatorsToAlt(this string value)
    {
        return value.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
    
    public static string ReplaceSeparatorsToDefault(this string value)
    {
        return value.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
    }
    
}