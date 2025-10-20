namespace SolidZip.Views;

public sealed partial class MainWindow : Window
{
    private const string ClosingMainWindowLogMessage = $"Closing {nameof(MainWindow)}";
    private const string MinimizingMainWindowLogMessage = $"Minimizing {nameof(MainWindow)}";
    private const string MaximizingMainWindowLogMessage = $"Maximizing {nameof(MainWindow)}";
    private const string NormalizingMainWindowLogMessage = $"Normalizing {nameof(MainWindow)}";

    private readonly ILogger<MainWindow> _logger;
    
    public MainWindow(ILogger<MainWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
    }

    private void Minimize(object sender, RoutedEventArgs e)
    {
        _logger.LogDebug(MinimizingMainWindowLogMessage);
        WindowState = WindowState.Minimized;
    }

    private void Restore(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            _logger.LogDebug(NormalizingMainWindowLogMessage);
            WindowState = WindowState.Normal;
        }
        else
        {
            _logger.LogDebug(MaximizingMainWindowLogMessage);
            WindowState = WindowState.Maximized;
        }
    }
    
    private void Close(object sender, RoutedEventArgs e)
    {
        _logger.LogDebug(ClosingMainWindowLogMessage);
        Close();
    }
    
}