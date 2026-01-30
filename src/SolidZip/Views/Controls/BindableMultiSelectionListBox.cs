namespace SolidZip.Views.Controls;

public sealed class BindableMultiSelectionListBox : ListBox, IMultiSelector
{
    public new static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(IList), typeof(BindableMultiSelectionListBox),
        new PropertyMetadata(default(IList)));

    public BindableMultiSelectionListBox()
    {
        SelectionMode = SelectionMode.Multiple;
        SelectionChanged += (_, args) =>
        {
            foreach (var item in args.AddedItems)
                SelectedItems.Add(item);
            foreach (var item in args.RemovedItems)
                SelectedItems.Remove(item);
        };
    }


    public new IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public void SetSelection(object item)
    {
        var listBoxItem = (ListBoxItem)item;
        listBoxItem.IsSelected = true;
        ApplyTemplate();
        UpdateLayout();
    }

    public void RemoveSelection(object item)
    {
        var listBoxItem = (ListBoxItem)item;
        listBoxItem.IsSelected = false;
        ApplyTemplate();
        UpdateLayout();
    }

    public IEnumerable<MultiSelectorItemInfo> GetItems()
    {
        return Enumerable.Range(0, Items.Count)
            .Select(i => ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem)
            .Where(listBoxItem => listBoxItem is not null)
            .Select(listBoxItem =>
            {
                var transform = listBoxItem!.TransformToAncestor(this)!;
                return new MultiSelectorItemInfo(
                    listBoxItem,
                    listBoxItem.IsSelected,
                    transform.Transform(new Point(0, 0)),
                    transform.Transform(new Point(listBoxItem.ActualWidth, 0)),
                    transform.Transform(new Point(0, listBoxItem.ActualHeight)),
                    transform.Transform(new Point(listBoxItem.ActualWidth, listBoxItem.ActualHeight))
                );
            });
    }
}