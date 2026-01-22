namespace SolidZip.Views;

public partial class StartupView : Window
{
    public StartupView()
    {
        InitializeComponent();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
    }
}