namespace SolidZip.Core.Utils;

public class ExtensionIconExtractor
{
    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

    public virtual  IconInfo Extract(string extension)
    {
        try
        {
            if (string.IsNullOrEmpty(extension))
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            var fileExtension = Path.HasExtension(extension)
                ? Path.GetExtension(extension)
                : extension;

            if (!fileExtension.StartsWith("."))
                fileExtension = "." + fileExtension;

            using var hKeyClassRoot = Registry.ClassesRoot;
            using var extensionKey = hKeyClassRoot.OpenSubKey(fileExtension);

            if (extensionKey is null)
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            var fileType = extensionKey.GetValue(string.Empty) as string;
            if (string.IsNullOrEmpty(fileType))
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            using var fileTypeKey = hKeyClassRoot.OpenSubKey(fileType);
            if (fileTypeKey is null)
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            using var defaultIconKey = fileTypeKey.OpenSubKey("DefaultIcon");
            if (defaultIconKey is null)
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            var iconLocation = defaultIconKey.GetValue(string.Empty) as string;
            if (string.IsNullOrEmpty(iconLocation))
                return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));

            var parts = iconLocation.Split(',');
            var iconPath = parts[0].Trim('"');

            var index = 0;
            if (parts.Length > 1)
                int.TryParse(parts[1], out index);

            var hIcon = ExtractIcon(IntPtr.Zero, iconPath, index);

            return hIcon != IntPtr.Zero
                ? new IconInfo(hIcon, hIcon => DestroyIcon(hIcon))
                : new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));
        }
        catch
        {
            return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));
        }
    }
}