namespace SolidZip.Views.MarkupExtensions;

public sealed class ResolveViewAndViewModelExtension : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var rootObjectProvider = serviceProvider.GetRequiredService<IRootObjectProvider>();
        var root = rootObjectProvider?.RootObject as FrameworkElement;
        try
        {
            var viewModelLocator = Ioc.Default.GetRequiredService<ViewModelLocator>();
            return viewModelLocator.GetDataContext(root.GetType().Name);
        }
        catch (Exception e)
        {
            Ioc.Default
                .GetRequiredService<ILogger<ResolveViewAndViewModelExtension>>()
                .LogError(e, "Exception occurred");
          
            return null;
        }
       
    }
    
    public void Okak(EventHandler name, string value)
    {
      
    }
}