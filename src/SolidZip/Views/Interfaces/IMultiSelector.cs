namespace SolidZip.Views.Interfaces;

public interface IMultiSelector
{
    public void SetSelection(object item);
    public void RemoveSelection(object item);
    public IEnumerable<MultiSelectorItemInfo> GetItems();
}