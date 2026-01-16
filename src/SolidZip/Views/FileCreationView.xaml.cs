namespace SolidZip.Views;

public partial class FileCreationView : Window
{
    public FileCreationView()
    {
        InitializeComponent();
    }
    
    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 1)
            DragMove();
    }
}