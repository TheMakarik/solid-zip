namespace SolidZip.Core.Utils;

public class AssociatedIconExtractor
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi, uint cbSizeFileInfo, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);
    
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

    private const uint ShgfiIcon = 0x100;
    private const uint ShgfiSmallicon = 0x1;

    public  IconInfo Extract(string path)
    {
        try
        {
            Shfileinfo shInfo = new Shfileinfo();
            uint flags = ShgfiIcon | ShgfiSmallicon;

            SHGetFileInfo(path, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), flags);

            return new IconInfo(shInfo.hIcon, (hIcon) => DestroyIcon(hIcon));
        }
        catch
        {
            return new IconInfo(IntPtr.Zero, (hIcon) => DestroyIcon(hIcon));
        }
    }
}