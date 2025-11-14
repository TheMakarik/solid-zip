namespace SolidZip.Views.MarkupExtensions;

public class ResolveViewAndViewModelExtension : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var rootObjectProvider = serviceProvider.GetRequiredService<IRootObjectProvider>();
        var root = rootObjectProvider?.RootObject as FrameworkElement;
        var viewModelLocator = Ioc.Default.GetRequiredService<ViewModelLocator>();
        return viewModelLocator.GetDataContext(root.GetType().Name);
    }
}