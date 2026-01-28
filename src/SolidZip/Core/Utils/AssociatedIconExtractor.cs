namespace SolidZip.Core.Utils;

public class AssociatedIconExtractor
{
    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_SMALLICON = 0x1;
    private const uint SHGFI_LARGEICON = 0x0;
    private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
    private const uint SHGFI_USEFILEATTRIBUTES = 0x10;

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi,
        uint cbSizeFileInfo, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public virtual IconInfo Extract(FileEntity fileEntity)
    {
        try
        {
            var path = fileEntity.Path;
            var shInfo = new Shfileinfo();
            var flags = SHGFI_ICON | SHGFI_SMALLICON;

            if (IsRootDrive(path) || Directory.Exists(path))
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                var fileAttributes = FILE_ATTRIBUTE_DIRECTORY;

                SHGetFileInfo(path, fileAttributes, ref shInfo, (uint)Marshal.SizeOf(shInfo), flags);
            }
            else
            {
                SHGetFileInfo(path, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), flags);
            }

            return new IconInfo(shInfo.hIcon, hIcon => DestroyIcon(hIcon));
        }
        catch
        {
            return new IconInfo(IntPtr.Zero, hIcon => DestroyIcon(hIcon));
        }
    }
    
    public virtual IconInfo Extract(string path)
    {
        return Extract(default(FileEntity) with { Path = path, IsDirectory = Directory.Exists(path)});
    }

    private bool IsRootDrive(string path)
    {
        if (path is [_, ':'])
            return true;
        return path.Length == 3 && path.EndsWith(":\\");
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct Shfileinfo
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
}