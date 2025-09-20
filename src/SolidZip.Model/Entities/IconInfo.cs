namespace SolidZip.Model.Entities;

public class IconInfo : IDisposable
{
    public IntPtr HIcon { get; }
    
    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public IconInfo(IntPtr hIcon)
    {
        HIcon = hIcon;
    }

    public void Dispose()
    {
        if (HIcon != IntPtr.Zero)
        {
            DestroyIcon(HIcon);
        }
    }
    
}