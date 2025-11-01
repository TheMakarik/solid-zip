namespace SolidZip.Deprecated;

public sealed class ViewModelLocator(IServiceProvider provider)
{
    public TView GetView<TView>() where TView : ContentControl
    {
        return provider.GetView<TView>();
    }

    public ContentControl GetView(ExplorerElementsView explorerElementsView)
    {
        return provider.GetView(explorerElementsView);
    }
}