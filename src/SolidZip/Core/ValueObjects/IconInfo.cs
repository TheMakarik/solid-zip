namespace SolidZip.Core.ValueObjects;

public class IconInfo(IntPtr hIcon, Action<IntPtr> destroy) : IDisposable
{
    public IntPtr HIcon { get; } = hIcon;

    public void Dispose()
    {
        if (HIcon != IntPtr.Zero) destroy(HIcon);
    }
}