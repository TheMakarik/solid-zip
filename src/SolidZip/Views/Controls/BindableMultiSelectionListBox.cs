
namespace SolidZip.Views.Controls;

public sealed class BindableMultiSelectionListBox : ListBox, IMultiSelector
{
    public new static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
        nameof(SelectedItems), typeof(IList), typeof(BindableMultiSelectionListBox),
        new PropertyMetadata(default(IList)));
    

    public new IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public BindableMultiSelectionListBox()
    {
        SelectionMode = SelectionMode.Multiple;
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
        var listBoxItem = (ListBoxItem)item;
        listBoxItem.IsSelected = true;
        this.ApplyTemplate();
        this.UpdateLayout();
    }

    public IEnumerable<MultiSelectorItemInfo> GetItems()
    {
        return from ListBoxItem listBoxItem
            in Items
            let transform = listBoxItem.TransformToAncestor(this)
            select new MultiSelectorItemInfo(
               Control: listBoxItem, 
               IsSelected: listBoxItem.IsSelected,
               TopLeft: transform.Transform(new Point(0, 0)),
               TopRight: transform.Transform(new Point(listBoxItem.ActualWidth, 0)),
               BottomLeft: transform.Transform(new Point(0, listBoxItem.ActualHeight)),
               BottomRight: transform.Transform(new Point(listBoxItem.ActualWidth, listBoxItem.ActualHeight))
           );
    }
}