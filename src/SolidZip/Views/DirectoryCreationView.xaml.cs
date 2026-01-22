namespace SolidZip.Views;

public partial class DirectoryCreationView : Window
{
    public DirectoryCreationView()
    {
        InitializeComponent();
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }


    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
    }
}