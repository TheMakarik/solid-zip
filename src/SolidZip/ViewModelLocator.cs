namespace SolidZip;

public sealed class ViewModelLocator(IServiceProvider provider)
{
    public TView GetView<TView>() where TView : ContentControl
    {
        return provider.GetView<TView>();
    }
}