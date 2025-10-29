namespace SolidZip.Views;

[INotifyPropertyChanged]
public partial class StartupWindow : Window
{
    [ObservableProperty] private string _progressText = string.Empty;
    [ObservableProperty] private double _progress;
    
    public StartupWindow()
    {
        InitializeComponent();
    }
    
    private void Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
        Application.Current.Shutdown();
    }
}