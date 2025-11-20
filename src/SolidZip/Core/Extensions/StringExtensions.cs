namespace SolidZip.Core.Extensions;

public static class StringExtensions
{
    public static FileEntity ToDirectoryFileEntity(this string path, bool rootDirectory = false)
    {
        if (rootDirectory)
            return new FileEntity(path, IsDirectory: true, default, default, null);
        
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

    public static string CutFromEnd(this string value, char charTillCut, char stopChar)
    {
        var tillCharWasFoundFlag = false;
        var lastIndex = value.Length - 1;
        var tillCharIndex = lastIndex;
        for (var i = lastIndex; i >= 0; i--)
        {
            if (value[i] == charTillCut)
            {
                if (!tillCharWasFoundFlag)
                    tillCharWasFoundFlag = true;
                tillCharIndex = i;
            }
            
            if (value[i] == stopChar && tillCharWasFoundFlag)
                return value[..tillCharIndex];
        }

        return value;
    }
}