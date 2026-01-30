namespace SolidZip.Views;

public partial class RequirePasswordView : Window
{
    public RequirePasswordView()
    {
        InitializeComponent();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
    }
}
