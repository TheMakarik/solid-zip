namespace SolidZip.Core.Common;

public class ViewModelLocator(IServiceProvider provider, ILogger<ViewModelLocator> logger)
{
    public TView Resolve<TView>() where TView : FrameworkElement
    {
        var dataContext = GetDataContext(typeof(TView)?.FullName ?? string.Empty);
        var view = provider.GetRequiredService<TView>();
        view.DataContext = dataContext;
        return view;
    }

    public object? GetDataContext(string viewName)
    {
        var viewModelName = viewName.Replace("View", "ViewModel");
        return provider.GetRequiredService(viewModelName);
    }
}