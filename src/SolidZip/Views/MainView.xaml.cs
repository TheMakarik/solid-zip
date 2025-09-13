namespace SolidZip.Views;

public sealed partial class MainView : Window
{
    private const string ClosingMainWindowLogMessage = $"Closing {nameof(MainView)}";
    private const string MinimizingMainWindowLogMessage = $"Minimizing {nameof(MainView)}";
    private const string MaximizingMainWindowLogMessage = $"Maximizing {nameof(MainView)}";
    private const string NormalizingMainWindowLogMessage = $"Normalizing {nameof(MainView)}";
    
    private readonly ILogger<MainView> _logger;
    
    public MainView(ILogger<MainView> logger)
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