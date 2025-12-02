using Material.Icons;
using Material.Icons.WPF;

namespace SolidZip.Views;


public sealed partial class MainView
{
    private record LuaLoadMenuItem(
        //DO NOT RENAME THIS PROPERTY, IT NEED TO BE CALLED TO DUE TO LUA CONVERSIONS
        // ReSharper disable once InconsistentNaming
        MenuItem menu_item
    );
    
    private const int MaxDotCountAtLoadingHeader = 3;
    
    private readonly ILuaUiData _uiData;
    private readonly ILuaEventRaiser _raiser;
    private readonly ILogger<MainView> _logger;
    private readonly ConcurrentBag<string> _loadedMenuItems = new();

    public MainView(ILuaUiData uiData, ILuaEventRaiser raiser, ILogger<MainView> logger)
    {
        InitializeComponent();
        _raiser = raiser;
        _uiData = uiData;
        _logger = logger;
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

    private void MenuItemsContentLoading_Loaded(object sender, RoutedEventArgs e)
    {
        var menuItem = (MenuItem)sender;
        var parentMenuItem = (MenuItem)ItemsControl.ItemsControlFromItemContainer(menuItem);
        
        if (_loadedMenuItems.Contains(parentMenuItem.Name))
            return;
        
        if (menuItem.HasItems && menuItem.Items.Count > 1)
            return;
        
       
        
        var dotCount = 0;
        var originalHeader = menuItem.Header?.ToString() ?? string.Empty;
        originalHeader = originalHeader.TrimEnd('.');
        
        var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
        
        dispatcherTimer.Tick += (_, _) =>
        {
            dotCount = (dotCount + 1) % (MaxDotCountAtLoadingHeader + 1);

            if (dotCount == 0)
                menuItem.Header = originalHeader;
            
            else
                menuItem.Header = originalHeader + new string('.', dotCount);

        };
        dispatcherTimer.Start();
      
        
        var parentName = parentMenuItem.Name.ToSnakeCase();
        Task.Run(async () =>
        {
            _logger.LogDebug("Loading menu-items for {menu}", parentName);
            var result = await _raiser.RaiseAsync<MenuItem, LuaLoadMenuItem>(parentName + "_loaded", new(parentMenuItem));
            await Dispatcher.InvokeAsync(() => 
            {
                parentMenuItem.Items.RemoveAt(parentMenuItem.Items.Count - 1); //Remove last item
                foreach (var item in result)
                {
                    item.Style = Application.Current.Resources["SzMenuItem"] as Style;
                    parentMenuItem.Items.Add(item);
                }
          
                
            });
        });
        
        _loadedMenuItems.Add(parentMenuItem.Name);
    }
    
  
    


  
}