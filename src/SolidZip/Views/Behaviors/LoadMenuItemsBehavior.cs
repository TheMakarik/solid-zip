using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SolidZip.Views.Behaviors;

public sealed class LoadMenuItemsBehavior : Behavior<MenuItem>
{
   
    private record MenuItemValue(
        //DO NOT RENAME THIS PROPERTY, IT'S CALLED SO DUE TO LUA CONVERSIONS, and by name will be loaded in lua
        // ReSharper disable once InconsistentNaming
        MenuItem menu_item
        );
    
    protected override void OnAttached()
    {
        AssociatedObject.SubmenuOpened += LoadItemsAsync;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.SubmenuOpened -= LoadItemsAsync;
        base.OnDetaching();
    }

  private async void LoadItemsAsync(object sender, RoutedEventArgs args)
{
    var menuItem = (MenuItem)sender;
    
    var eventName = menuItem.Name.ToSnakeCase() + "_opened";
    
    var loaderMenuIndex = menuItem.Items.Count; //last index + 1
    var loaderMenuItem = new MenuItem();
    
    loaderMenuItem.Triggers.Clear();
    
    var iconContainer = new Grid
    {
        Width = 16,
        Height = 16,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };
    
    var loadingAnimation = new Ellipse
    {
        Width = 16,
        Height = 16,
        Stroke = Brushes.Gray,
        StrokeThickness = 2,
        StrokeDashArray = [1, 5], 
        StrokeDashCap = PenLineCap.Round
    };

    iconContainer.Children.Add(loadingAnimation);
    loaderMenuItem.Icon = iconContainer;
    
    var rotateTransform = new RotateTransform();
    loadingAnimation.RenderTransform = rotateTransform;
    loadingAnimation.RenderTransformOrigin = new Point(0.5, 0.5);
    var rotationAnimation = new DoubleAnimation
    {
        From = 0,
        To = 360,
        Duration = TimeSpan.FromSeconds(1),
        RepeatBehavior = RepeatBehavior.Forever
    };

    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);
    menuItem.Items.Add(loaderMenuItem);

    var eventRaiser = Ioc.Default.GetRequiredService<ILuaEventRaiser>();
    var task = eventRaiser
        .RaiseAsync<MenuItem, MenuItemValue>(eventName, new(menuItem));
    
    var result = await task;
    
    menuItem.Items.RemoveAt(loaderMenuIndex);

    foreach (var item in result)
        menuItem.Items.Add(item);
}
}