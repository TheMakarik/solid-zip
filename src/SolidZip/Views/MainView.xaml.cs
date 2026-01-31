namespace SolidZip.Views;

public sealed partial class MainView
{
    private const double SelectionBorderDefaultSize = 0.0d;

    private static readonly TimeSpan
        WaitToShowSelectionBorder = TimeSpan.FromMilliseconds(70);

    private static readonly TimeSpan WaitToResize = TimeSpan.FromTicks(5);

    private readonly LuaMenuItemsLoader _luaMenuItemsLoader;
    private readonly DispatcherTimer _resizeSelectionBorderTimer;
    private readonly DispatcherTimer _showSelectionBorderTimer;
    private IMultiSelector? _currentSelector;
    private MultiSelectorItemInfo[] _items;
    private Point? _startBorderPosition;
    private readonly HashSet<object> _selectedDuringDrag = new();

    private Point? _startMousePosition;
    private readonly ILogger<MainView> _logger;

    public MainView(LuaMenuItemsLoader luaMenuItemsLoader, IMessenger messenger, ILogger<MainView> logger)
    {
        InitializeComponent();
        SetSelectionBorderDefaultSize();
        _logger = logger;
        _luaMenuItemsLoader = luaMenuItemsLoader;

        _resizeSelectionBorderTimer = new DispatcherTimer { Interval = WaitToResize };
        _resizeSelectionBorderTimer.Tick += (_, _) => ResizeSelectionBorder(Mouse.GetPosition(ExplorerContent));

        _showSelectionBorderTimer = new DispatcherTimer { Interval = WaitToShowSelectionBorder };
        _showSelectionBorderTimer.Tick += (_, _) =>
        {
            _logger.LogDebug("Selection was started at mouse cords: {cords}", Mouse.GetPosition(ExplorerContent));
            SelectionBorder.Visibility = Visibility.Visible;
            _showSelectionBorderTimer.Stop();
            _resizeSelectionBorderTimer.Start();
        };
    }

    private void Minimize(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void Restore(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;

    private void Close(object sender, RoutedEventArgs e) =>
        Close();

    private void ShowSelectionBorder(object sender, MouseButtonEventArgs e)
    {
        var initialMousePos = Mouse.GetPosition(ExplorerContent);
        _startMousePosition = initialMousePos;

        var left = initialMousePos.X - SelectionBorder.ActualWidth / 2;
        var top = initialMousePos.Y - SelectionBorder.ActualHeight / 2;
        Canvas.SetLeft(SelectionBorder, left);
        Canvas.SetTop(SelectionBorder, top);
        _startBorderPosition = new Point(left, top);
        _currentSelector = FindVisualChild<IMultiSelector>(ExplorerContent);
        _items = _currentSelector.GetItems().ToArray();
        _selectedDuringDrag.Clear();

        _showSelectionBorderTimer.Start();
        e.Handled = false; 
    }

    private void LoadLuaMenuItems(object sender, RoutedEventArgs e) =>
        _luaMenuItemsLoader.LoadMenuItems(
            (MenuItem)sender,
            Application.Current.Resources["SzMenuItem"] as Style,
            Application.Current.Resources["SzLoadingMenuItem"] as Style);

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

        var currentSelectionRect = new Rect(
            Math.Min(_startMousePosition.Value.X, currentMousePosition.X),
            Math.Min(_startMousePosition.Value.Y, currentMousePosition.Y),
            Math.Abs(currentMousePosition.X - _startMousePosition.Value.X),
            Math.Abs(currentMousePosition.Y - _startMousePosition.Value.Y)
        );

        UpdateSelection(currentSelectionRect);
    }

    private void UpdateSelection(Rect selectionRect)
    {
        if (_currentSelector is null)
        {
            _logger.LogError("Cannot find current selector");
            return;
        }

        foreach (var item in _items)
        {
            var isInRect = IsInSelectionRect(item, selectionRect);
            
            switch (isInRect)
            {
                case false when _selectedDuringDrag.Contains(item.Control):
                    _currentSelector.RemoveSelection(item.Control);
                    break;
                case true when !item.IsSelected:
                    _currentSelector.SetSelection(item.Control);
                    _selectedDuringDrag.Add(item.Control);
                    break;
            }
        }
    }

    private bool IsInSelectionRect(MultiSelectorItemInfo item, Rect selectionRect)
    {
        var maxX = Math.Max(item.TopLeft.X, item.BottomRight.X);
        var minX = Math.Min(item.TopLeft.X, item.BottomRight.X);
        var maxY = Math.Max(item.TopLeft.Y, item.BottomRight.Y);
        var minY = Math.Min(item.TopLeft.Y, item.BottomRight.Y);

        var itemRect = new Rect(new Point(minX, minY), new Point(maxX, maxY));

        return selectionRect.IntersectsWith(itemRect);
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
        _logger.LogDebug("Selection was stopped with cords: {cords}", Mouse.GetPosition(ExplorerContent));
        _items = [];
        _selectedDuringDrag.Clear();
    }

    private static T FindVisualChild<T>(DependencyObject parent) where T : class
    {
        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T found)
                return found;

            var childResult = FindVisualChild<T>(child);
            return childResult;
        }

        throw new InvalidOperationException($"Could not find child of type {typeof(T)}");
    }
}