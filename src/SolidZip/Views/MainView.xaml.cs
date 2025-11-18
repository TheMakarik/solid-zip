namespace SolidZip.Views;

public sealed partial class MainView 
{
    public MainView()
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
}