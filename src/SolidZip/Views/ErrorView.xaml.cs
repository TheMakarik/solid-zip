namespace SolidZip.Views;

public partial class ErrorView
{
    private readonly ILuaEventRaiser _eventRaiser;

    public ErrorView(ILuaEventRaiser eventRaiser)
    {
        InitializeComponent();
        _eventRaiser = eventRaiser;
    }
    
    private async void Close(object sender, RoutedEventArgs e)
    {
       await _eventRaiser.RaiseAsync("error_window_closed");
       Application.Current.Shutdown();
    }

    private void ErrorView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
       DragMove();
    }
}