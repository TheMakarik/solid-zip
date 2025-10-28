namespace SolidZip;

public static class ViewModelLocatorExtensions
{
    private const string ResolvingViewAndViewModelLogMessage = "Resolving {view} and {viewModel}";
    
    private static readonly ConcurrentDictionary<Type, Type> ViewAndViewModelMap = new();
    
    public static IServiceCollection Bind<TView, TViewModel>(this IServiceCollection services, ExplorerElementsView? view = null) 
        where TView : ContentControl
        where TViewModel : ViewModelBase
    {
        if (view is null)
        {
            services.TryAddTransient<TView>();
            services.TryAddTransient<TViewModel>();
        }
        else
        {
            services.AddKeyedTransient<ContentControl, TView>(view);
            services.TryAddSingleton<TViewModel>();
        }
        
        ViewAndViewModelMap
            .TryAdd(typeof(TView), typeof(TViewModel));
        
        return services;
    }

    public static TView GetView<TView>(this IServiceProvider provider) 
        where TView :  ContentControl
    {
        var view = provider.GetRequiredService<TView>();
        var viewModel = provider.GetRequiredService(ViewAndViewModelMap[typeof(TView)]);
        
        provider.GetRequiredService<ILogger<Application>>()
            .LogDebug(ResolvingViewAndViewModelLogMessage, view.GetType().Name, viewModel.GetType().Name);
        
        view.DataContext = viewModel;
        return view;
    }

    public static ContentControl GetView(this IServiceProvider provider, ExplorerElementsView explorerElementsView)
    {
        var view = provider.GetRequiredKeyedService<ContentControl>(explorerElementsView);
        var viewModel = provider.GetRequiredService(ViewAndViewModelMap[view.GetType()]);
        
        provider.GetRequiredService<ILogger<Application>>()
            .LogDebug(ResolvingViewAndViewModelLogMessage, view.GetType().Name, viewModel.GetType().Name);
        
        view.DataContext = viewModel;
        return view;
    
    }
}