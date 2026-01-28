namespace SolidZip.Core.Utils;

public class ExtensionIconExtractor
{
    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

    private static readonly IconInfo EmptyIcon = new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

    public virtual IconInfo Extract(FileEntity entity)
    {
        if (entity.IsDirectory)
            return GetFolderIcon();

        var extension = entity.Path;

        if (string.IsNullOrEmpty(extension))
            return GetDefaultFileIcon();

        var fileExtension = NormalizeExtension(extension);

        return ExtractIconFromRegistry(fileExtension) ?? GetDefaultFileIcon();
    }
    
    public virtual IconInfo Extract(string filePath)
    {
       return Extract(default(FileEntity) with { Path = filePath, IsDirectory = Directory.Exists(filePath) });
    }

    private IconInfo GetFolderIcon()
    {
        var hIcon = ExtractIcon(IntPtr.Zero, "imageres.dll", 3);
        if (hIcon != IntPtr.Zero)
            return new IconInfo(hIcon, hIcon => DestroyIcon(hIcon));

        hIcon = ExtractIcon(IntPtr.Zero, "shell32.dll", 3);
        return hIcon != IntPtr.Zero 
            ? new IconInfo(hIcon, hIcon => DestroyIcon(hIcon))
            : GetDefaultFileIcon();
    }

    private IconInfo GetDefaultFileIcon()
    {
        var hIcon = ExtractIcon(IntPtr.Zero, "shell32.dll", 0);
        return hIcon != IntPtr.Zero 
            ? new IconInfo(hIcon, hIcon => DestroyIcon(hIcon))
            : EmptyIcon;
    }

    private IconInfo? ExtractIconFromRegistry(string fileExtension)
    {
        try
        {
            using var extensionKey = Registry.ClassesRoot.OpenSubKey(fileExtension);
            if (extensionKey is null)
                return null;

            var fileType = extensionKey.GetValue(string.Empty) as string;
            if (string.IsNullOrEmpty(fileType))
                return null;

            using var fileTypeKey = Registry.ClassesRoot.OpenSubKey(fileType);
            if (fileTypeKey is null)
                return null;

            using var defaultIconKey = fileTypeKey.OpenSubKey("DefaultIcon");
            if (defaultIconKey is null)
                return null;

            var iconLocation = defaultIconKey.GetValue(string.Empty) as string;
            if (string.IsNullOrEmpty(iconLocation))
                return null;

            var (iconPath, index) = ParseIconLocation(iconLocation);
            var hIcon = ExtractIcon(IntPtr.Zero, iconPath, index);
            
            return hIcon != IntPtr.Zero 
                ? new IconInfo(hIcon, hIcon => DestroyIcon(hIcon))
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return extension;

        var fileExtension = Path.HasExtension(extension) 
            ? Path.GetExtension(extension) 
            : extension;

        return fileExtension.StartsWith(".") 
            ? fileExtension 
            : "." + fileExtension;
    }

    private static (string Path, int Index) ParseIconLocation(string iconLocation)
    {
        var parts = iconLocation.Split(',');
        var iconPath = parts[0].Trim('"');

        var index = 0;
        if (parts.Length > 1)
            int.TryParse(parts[1], out index);

        return (iconPath, index);
    }
}