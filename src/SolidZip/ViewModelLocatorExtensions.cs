using System.Collections.Concurrent;
using System.Diagnostics;

namespace SolidZip;

public static class ViewModelLocatorExtensions
{
    private const string ResolvingViewAndViewModelLogMessage = "Resolving {view} and {viewModel}";
    
    private static readonly ConcurrentDictionary<Type, Type> ViewAndViewModelMap = new();
    
    public static IServiceCollection Bind<TView, TViewModel>(this IServiceCollection services) 
        where TView : ContentControl
        where TViewModel : ViewModelBase
    {
        services
            .AddTransient<TView>()
            .AddSingleton<TViewModel>();
        
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
}