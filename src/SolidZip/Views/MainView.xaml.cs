
namespace SolidZip.Views;

public sealed partial class MainView
{
    private readonly LuaMenuItemsLoader _luaMenuItemsLoader;

    public MainView(LuaMenuItemsLoader luaMenuItemsLoader)
    {
        InitializeComponent();
        _luaMenuItemsLoader = luaMenuItemsLoader;
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

    private void LoadLuaMenuItems(object sender, RoutedEventArgs e)
    {
        _luaMenuItemsLoader.LoadMenuItems(
            (MenuItem)sender,
            Application.Current.Resources["SzMenuItem"] as Style,
            Application.Current.Resources["SzLoadingMenuItem"] as Style);
    }
    
    
    
}