namespace SolidZip.Views;

public partial class ExtractArchiveView : Window
{
    public ExtractArchiveView()
    {
        InitializeComponent();
    }

    private void ExtractArchiveView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
       this.DragMove();
    }
}