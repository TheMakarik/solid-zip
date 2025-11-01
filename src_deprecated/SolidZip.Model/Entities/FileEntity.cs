namespace SolidZip.Model.Entities;

public record struct FileEntity(string Path, bool IsDirectory, bool IsArchiveEntry = false)
{
    //Some strange things happens, when trying to open directory
    //chance to invoke command with string instead of FileEntity exists
    public static explicit operator FileEntity(string path)
    {
        return new FileEntity(path, Directory.Exists(path));
    }
}