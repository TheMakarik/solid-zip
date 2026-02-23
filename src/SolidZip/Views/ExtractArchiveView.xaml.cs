namespace SolidZip.Views;

public partial class ExtractArchiveView : Window
{
    public ExtractArchiveView()
    {
        InitializeComponent();
    }

    private void ExtractArchiveView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
       this.DragMove();
    }

    private void Close(object sender, RoutedEventArgs e)
    {
       Close();
    }

    private void Minimize(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void Restore(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
}