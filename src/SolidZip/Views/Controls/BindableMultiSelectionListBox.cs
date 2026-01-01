namespace SolidZip.Views.Controls;

public sealed class BindableMultiSelectionListBox : ListBox
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
}