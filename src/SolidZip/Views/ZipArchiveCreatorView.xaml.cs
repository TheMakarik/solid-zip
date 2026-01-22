namespace SolidZip.Views;

public partial class ZipArchiveCreatorView : Window
{
    public ZipArchiveCreatorView()
    {
        InitializeComponent();
    }

    private void Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Restore(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}