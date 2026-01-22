using SolidZip.Views.Interfaces;

namespace SolidZip.Views.Controls;

public sealed class BindableMultiSelectionDataGrid : DataGrid, IMultiSelector
{
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(IList), typeof(BindableMultiSelectionDataGrid),
        new PropertyMetadata(default(IList)));

    public new IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public BindableMultiSelectionDataGrid()
    {
        SelectionMode = DataGridSelectionMode.Extended;
        SelectionUnit = DataGridSelectionUnit.FullRow;
        
        this.SelectionChanged += (_, args) =>
        {
            foreach (var item in args.AddedItems)
                SelectedItems.Add(item);
            foreach (var item in args.RemovedItems)
                SelectedItems.Remove(item);
        };
    }

    public void SetSelection(object item)
    {
        var rowToSelect = item as DataGridRow;
        rowToSelect?.IsSelected = true;
    }

    public IEnumerable<MultiSelectorItemInfo> GetItems()
    {
        return from DataGridRow row in Items
            let transform = row.TransformToAncestor(this)
            select new MultiSelectorItemInfo(
                Control: row,
                IsSelected: row.IsSelected,
                TopLeft: transform.Transform(new Point(0, 0)),
                TopRight: transform.Transform(new Point(row.ActualWidth, 0)),
                BottomLeft: transform.Transform(new Point(0, row.ActualHeight)),
                BottomRight: transform.Transform(new Point(row.ActualWidth, row.ActualHeight))
            );
    }
}