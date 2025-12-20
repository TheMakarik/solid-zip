namespace SolidZip.Views.Behaviors;

public class ControlPlusMouseWheelBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty CommandProperty = 
        DependencyProperty.Register(
            nameof(Command), 
            typeof(ICommand), 
            typeof(ControlPlusMouseWheelBehavior), 
            new PropertyMetadata(default(ICommand)));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        
        AssociatedObject.Focusable = true;
        AssociatedObject.Focus();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        base.OnDetaching();
    }
    
    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Control)
            return;
        
        e.Handled = true;
        var direction = e.Delta > 0 ? 1 : -1;
        
        if (Command?.CanExecute(direction) == true)
            Command.Execute(direction);
    }
    
}