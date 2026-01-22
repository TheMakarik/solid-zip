namespace SolidZip.Views;

public partial class SettingsView
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void CloseSettings(object sender, RoutedEventArgs e)
    {
        Close();
    }


    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
    }
}