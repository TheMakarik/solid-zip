using SolidZip.Views.ValueObjects;

namespace SolidZip.Views.Interfaces;

public interface IMultiSelector
{
    public void SetSelection(object item);
    public IEnumerable<MultiSelectorItemInfo> GetItems();
}