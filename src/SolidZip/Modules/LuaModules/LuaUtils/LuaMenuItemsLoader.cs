namespace SolidZip.Modules.LuaModules.LuaUtils;

public class LuaMenuItemsLoader(StrongTypedLocalizationManager localizationManager, ILuaEventRaiser raiser, ILogger<LuaMenuItemsLoader> logger)
{
    private const int MaxDotCountAtLoadingHeader = 3;
    private const double DelayToChangeDot = 1.5d;
    private readonly ConcurrentBag<string> _loadedMenuItems = [];
    
    public void LoadMenuItems(ItemsControl menuItem, Style? styleToUse, Style loadingStyle, object? args = null)
    {
        var loadingMenuItem = new MenuItem
        {
            Style = loadingStyle
        };

        if (_loadedMenuItems.Contains(menuItem.Name))
            return;
        
        _loadedMenuItems.Add(menuItem.Name);
        
        var dotCount = 0;
        loadingMenuItem.Header = localizationManager.Loading;
        var originalHeader = loadingMenuItem.Header.ToString() ?? string.Empty;
        originalHeader = originalHeader.TrimEnd('.');
        
        var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(DelayToChangeDot) };
        
        dispatcherTimer.Tick += (_, _) =>
        {
            dotCount = (dotCount + 1) % (MaxDotCountAtLoadingHeader + 1);

            if (dotCount == 0)
                loadingMenuItem.Header = originalHeader;
            
            else
                loadingMenuItem.Header = originalHeader + new string('.', dotCount);
        };
        dispatcherTimer.Start();
      
        
        var parentName = menuItem.Name.ToSnakeCase();
        Task.Factory.StartNew(async () =>
        {
            logger.LogDebug("Loading menu-items for {menu}", parentName);
            var result =
                await raiser.RaiseAsync<MenuItem, LuaLoadMenuItem>(parentName + "_loaded", new(menuItem, args));
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                menuItem.Items.Remove(loadingMenuItem);
                
                if(!menuItem.Items.Contains(loadingMenuItem))
                    logger.LogDebug("\"Loading menu-items\" for {menu} was deleted successfully", parentName);
                else
                    logger.LogError("\"Loading menu-items\" for {menu} was not deleted", parentName);
                foreach (var item in result)
                {
                    item.Style = styleToUse; 
                    menuItem.Items.Add(item);
                }
            });
        }, TaskCreationOptions.LongRunning);
    }
}