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

    public static string GetExtensionFromEnd(this string path)
    {
        var lastIndex = path.Length - 1;
        for (var i = lastIndex; i >= 0; i--)
        {
            if (path[i] == '.')
                return path[i..];
            if (path[i] == Path.DirectorySeparatorChar)
                break;
        }

        return string.Empty;
    }

    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = new StringBuilder();
        result.Append(char.ToLower(value[0]));

        for (var i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(value[i]));
            }
            else
                result.Append(value[i]);
        }

        return result.ToString();
    }
}