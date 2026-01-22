namespace SolidZip.Views;

public sealed partial class MainView
{
    private const double SelectionBorderDefaultSize = 0.0d;

    private static readonly TimeSpan
        WaitToShowSelectionBorder = TimeSpan.FromMilliseconds(10); //For make less unexpected loading of SelectionBorder

    private static readonly TimeSpan WaitToResize = TimeSpan.FromTicks(5);

    private readonly LuaMenuItemsLoader _luaMenuItemsLoader;
    private readonly DispatcherTimer _resizeSelectionBorderTimer;
    private readonly DispatcherTimer _showSelectionBorderTimer;
    private IMultiSelector? _currentSelector;
    private Point? _startBorderPosition;

    private Point? _startMousePosition;

    public MainView(LuaMenuItemsLoader luaMenuItemsLoader)
    {
        InitializeComponent();
        SetSelectionBorderDefaultSize();
        _luaMenuItemsLoader = luaMenuItemsLoader;

        _resizeSelectionBorderTimer = new DispatcherTimer { Interval = WaitToResize };
        _resizeSelectionBorderTimer.Tick += (_, _) => ResizeSelectionBorder(Mouse.GetPosition(ExplorerContent));

        _showSelectionBorderTimer = new DispatcherTimer { Interval = WaitToShowSelectionBorder };
        _showSelectionBorderTimer.Tick += (_, _) =>
        {
            SelectionBorder.Visibility = Visibility.Visible;
            _showSelectionBorderTimer.Stop();
            _resizeSelectionBorderTimer.Start();
        };
    }

    private void Minimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Restore(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ShowSelectionBorder(object sender, MouseButtonEventArgs e)
    {
        var initialMousePos = Mouse.GetPosition(ExplorerContent);
        _startMousePosition = initialMousePos;

        var left = initialMousePos.X - SelectionBorder.ActualWidth / 2;
        var top = initialMousePos.Y - SelectionBorder.ActualHeight / 2;
        Canvas.SetLeft(SelectionBorder, left);
        Canvas.SetTop(SelectionBorder, top);
        _startBorderPosition = new Point(left, top);

        _showSelectionBorderTimer.Start();
        _currentSelector = (IMultiSelector)ExplorerContent.Template.FindName("ROOT_Content", ExplorerContent);
    }


    private void LoadLuaMenuItems(object sender, RoutedEventArgs e)
    {
        _luaMenuItemsLoader.LoadMenuItems(
            (MenuItem)sender,
            Application.Current.Resources["SzMenuItem"] as Style,
            Application.Current.Resources["SzLoadingMenuItem"] as Style);
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            DragMove();
    }

    private void ResizeSelectionBorder(Point currentMousePosition)
    {
        if (!_startMousePosition.HasValue || !_startBorderPosition.HasValue)
            return;

        if (currentMousePosition.X == 0 || currentMousePosition.Y == 0)
            return;

        var deltaX = currentMousePosition.X - _startMousePosition.Value.X;
        var deltaY = currentMousePosition.Y - _startMousePosition.Value.Y;

        var newWidth = Math.Max(SelectionBorderDefaultSize, Math.Abs(deltaX));
        var newHeight = Math.Max(SelectionBorderDefaultSize, Math.Abs(deltaY));

        var newLeft = _startBorderPosition.Value.X;
        var newTop = _startBorderPosition.Value.Y;

        if (deltaX < 0)
            newLeft = _startBorderPosition.Value.X + deltaX;

        if (deltaY < 0)
            newTop = _startBorderPosition.Value.Y + deltaY;

        Canvas.SetLeft(SelectionBorder, newLeft);
        Canvas.SetTop(SelectionBorder, newTop);

        SelectionBorder.Width = newWidth;
        SelectionBorder.Height = newHeight;
    }

    private void SetSelectionBorderDefaultSize()
    {
        SelectionBorder.Height = SelectionBorderDefaultSize;
        SelectionBorder.Width = SelectionBorderDefaultSize;
    }

    private void HideSelectionBorder(object sender, MouseButtonEventArgs e)
    {
        _showSelectionBorderTimer.Stop();
        _resizeSelectionBorderTimer.Stop();
        SelectionBorder.Visibility = Visibility.Collapsed;
        SetSelectionBorderDefaultSize();
        _currentSelector = null;
        _startMousePosition = null;
        _startBorderPosition = null;
    }
}