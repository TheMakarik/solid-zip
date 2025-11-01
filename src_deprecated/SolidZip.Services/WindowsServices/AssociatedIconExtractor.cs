namespace SolidZip.Services.WindowsServices;

internal sealed class AssociatedIconExtractor : IAssociatedIconExtractor
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_SMALLICON = 0x1;

    public IconInfo Extract(string path)
    {
        try
        {
            SHFILEINFO shInfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_SMALLICON;

            SHGetFileInfo(path, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), flags);

            return new IconInfo(shInfo.hIcon);
        }
        catch
        {
            return new IconInfo(IntPtr.Zero);
        }
    }
}