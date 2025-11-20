namespace SolidZip.Core.Utils;

public sealed class ExtensionIconExtractor
{
    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);
    
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
    
    public IconInfo Extract(string path)
    {
        using var hKeyClassRoot = Registry.ClassesRoot;
        using var iconKey = hKeyClassRoot.OpenSubKey(Path.GetExtension(path));

        if (iconKey is null)
            return new IconInfo(IntPtr.Zero, (hIcon) => DestroyIcon(hIcon));

        var iconLocation = iconKey.GetValue(string.Empty) as string;
            
        if (string.IsNullOrEmpty(iconLocation)) 
            return new IconInfo(IntPtr.Zero, (hIcon) => DestroyIcon(hIcon));

        var parts = iconLocation.Split(",");
        var iconPath = parts.First().Trim('"');
        
        var index = 0;
        if (parts.Length > 1)
            int.TryParse(parts[1], out index);
        
        var hIcon = ExtractIcon(IntPtr.Zero, path, index);
        return new IconInfo(hIcon, (iconPtr) => DestroyIcon(iconPtr));
    }
}