namespace SolidZip.Views.Controls;

public sealed class BindableMultiSelectionDataGrid : DataGrid, IMultiSelector
{
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(IList), typeof(BindableMultiSelectionDataGrid),
        new PropertyMetadata(default(IList)));

    public BindableMultiSelectionDataGrid()
    {
        SelectionMode = DataGridSelectionMode.Extended;
        SelectionUnit = DataGridSelectionUnit.FullRow;

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
        var rowToSelect = item as DataGridRow;
        rowToSelect?.IsSelected = true;
        this.UpdateLayout();
        this.ApplyTemplate();
    }

    public void RemoveSelection(object item)
    {
        var rowToSelect = item as DataGridRow;
        rowToSelect?.IsSelected = false;
        this.UpdateLayout();
        this.ApplyTemplate();
    }

    public IEnumerable<MultiSelectorItemInfo> GetItems()
    {
        return from DataGridRow row in Items
            let transform = row.TransformToAncestor(this)
            select new MultiSelectorItemInfo(
                row,
                row.IsSelected,
                transform.Transform(new Point(0, 0)),
                transform.Transform(new Point(row.ActualWidth, 0)),
                transform.Transform(new Point(0, row.ActualHeight)),
                transform.Transform(new Point(row.ActualWidth, row.ActualHeight))
            );
    }
}