namespace SolidZip.Core.Common;

public sealed class ViewModelLocator(IServiceProvider provider, ILogger<ViewModelLocator> logger)
{
    public object? GetDataContext(string viewName)
    {
        var viewModelName = viewName.Replace("View", "ViewModel");
        logger.LogDebug("Resolving {view} and {viewModel}", viewName, viewModelName);
        return provider.GetRequiredService(viewModelName);
    }
}