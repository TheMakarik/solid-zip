namespace SolidZip;

public static class ViewModelLocatorExtensions
{
    private const string ResolvingViewAndViewModelLogMessage = "Resolving {view} and {viewModel}";
    
    private static readonly ConcurrentDictionary<Type, Type> ViewAndViewModelMap = new();
    
    public static IServiceCollection Bind<TView, TViewModel>(this IServiceCollection services, ServiceLifetime viewModelLifetime = ServiceLifetime.Singleton, ExplorerElementsView? view = null) 
        where TView : ContentControl
        where TViewModel : ViewModelBase
    {
        services 
            .AddView<TView>(view)
            .AddViewModel<TViewModel>(viewModelLifetime);
        
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
    
    private static IServiceCollection AddView<TView>(this IServiceCollection services, ExplorerElementsView? view) where TView : ContentControl
    {
        if (view is null)
            services.TryAddTransient<TView>();
        else
            services.AddKeyedTransient<ContentControl, TView>(view);
        return services;
    }
    
    private static IServiceCollection AddViewModel<TViewModel>(this  IServiceCollection services, ServiceLifetime viewModelLifetime) where TViewModel : ViewModelBase
    {
        switch (viewModelLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<TViewModel>();
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped<TViewModel>();
                break;
            default:
                services.AddTransient<TViewModel>();
                break;
        }

        return services;
    }
}