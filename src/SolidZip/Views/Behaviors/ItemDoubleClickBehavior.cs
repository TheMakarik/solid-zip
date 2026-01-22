namespace SolidZip.Views.Behaviors;

public sealed class ItemDoubleClickBehavior : Behavior<ItemsControl>
{
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(ItemDoubleClickBehavior), new PropertyMetadata(default(ICommand)));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += HandlePreviewMouseDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseLeftButtonDown -= HandlePreviewMouseDown;
    }

    private void HandlePreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var source = e.OriginalSource as DependencyObject;
            var itemData = GetItemDataFromSource(source);

            if (itemData is not null && Command is not null && Command.CanExecute(itemData))
            {
                Command.Execute(itemData);
                e.Handled = true;
            }
        }
    }

    private object? GetItemDataFromSource(DependencyObject? source)
    {
        while (source != null && source != AssociatedObject)
        {
            if (source is DataGridCell dataGridCell)
            {
                var dataContext = dataGridCell.DataContext;
                if (dataContext != null && dataContext != AssociatedObject.DataContext)
                    return dataContext;

                var row = FindVisualParent<DataGridRow>(dataGridCell);
                if (row?.DataContext != null && row.DataContext != AssociatedObject.DataContext)
                    return row.DataContext;
            }

            if (source is DataGridRow dataGridRow &&
                dataGridRow.DataContext != null &&
                dataGridRow.DataContext != AssociatedObject.DataContext)
                return dataGridRow.DataContext;

            if (source is ListBoxItem listBoxItem)
            {
                var item = AssociatedObject.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                if (item is not null && item != AssociatedObject.DataContext)
                    return item;

                if (listBoxItem.DataContext is not null && listBoxItem.DataContext != AssociatedObject.DataContext)
                    return listBoxItem.DataContext;
            }

            if (VisualTreeHelper.GetParent(source) == AssociatedObject)
                if (source is FrameworkElement element &&
                    element.DataContext is not null &&
                    element.DataContext != AssociatedObject.DataContext)
                    return element.DataContext;

            if (source is ContentPresenter contentPresenter &&
                contentPresenter.Content is not null &&
                contentPresenter.Content != AssociatedObject.DataContext)
                return contentPresenter.Content;

            source = VisualTreeHelper.GetParent(source);
        }

        return null;
    }

    private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent is not null && !(parent is T))
            parent = VisualTreeHelper.GetParent(parent);
        return parent as T;
    }
}