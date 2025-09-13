using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SolidZip.Converters;

public class StringToImageSourceConverter : IValueConverter
{
    public static StringToImageSourceConverter Instance { get; } = new StringToImageSourceConverter();
    
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);
    
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
    private const uint SHGFI_LARGEICON = 0x0;
    private const uint SHGFI_SMALLICON = 0x1;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(value is string path) || !File.Exists(path) && !Directory.Exists(path))
            return null;

        try
        {
            SHFILEINFO shInfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_SMALLICON; 

            IntPtr result = SHGetFileInfo(path, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), flags);

            if (shInfo.hIcon != IntPtr.Zero)
            {
                ImageSource? imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    shInfo.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                
                DestroyIcon(shInfo.hIcon); 
                return imageSource;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}