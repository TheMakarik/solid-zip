using Microsoft.VisualBasic;
using SolidZip.Modules.LuaModules.LuaUtils;

namespace SolidZip.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache<T>(this IServiceCollection services, Action<T> expandAction) where T : class
    {
        var cache = new SharedCache<T>();
        cache.AddExpandAction(expandAction);
        return services.AddSingleton(cache);
    }

    public static IServiceCollection Configure<T>(this IServiceCollection services, IConfigurationManager configuration) where T : class
    {
        return services.Configure<T>(configuration.GetSection(typeof(T).Name));
    }
    
    public static IServiceCollection AddViewModelLocator(this IServiceCollection services)
    {
        return services.AddSingleton<ViewModelLocator>();
    }
    
    public static IServiceCollection AddPathsUtils(this IServiceCollection services)
    {
        return services
            .AddSingleton<PathFormatter>()
            .AddSingleton<PathsCollection>();
    }
    
    public static IServiceCollection AddAppData(this IServiceCollection services)
    {
        return services
            .AddSingleton<IUserJsonManager, UserJsonManager>()
            .AddTransient<IUserJsonCreator, UserJsonCreator>();
    }

    public static IServiceCollection AddLua(this IServiceCollection services)
    {
        return services
            .AddTransient<ILuaEventLoader, LuaEventLoader>()
            .AddSingleton<ILuaEvents, LuaEvents>()
            .AddSingleton<LuaMenuItemsLoader>()
            .AddSingleton<ILuaEventRaiser, LuaEventRaiser>()
            .AddSingleton<ILuaUiData, LuaUiData>()
            .AddSingleton<ILuaShared, LuaShared>()
            .AddSingleton<ILuaDebugConsole, LuaDebugConsole>()
            .AddSingleton<MaterialIconLuaLoader>()
            .AddSingleton<LuaEventRedirector>()
            .AddSingleton<ILuaGlobalsLoader, LuaGlobalsLoader>();
    }

    public static IServiceCollection AddWin32(this IServiceCollection services)
    {
        return services
            .AddSingleton<WindowsExplorer>()
            .AddSingleton<AssociatedIconExtractor>()
            .AddSingleton<ExtensionIconExtractor>()
            .AddSingleton<ConsoleAttacher>();
    }
    
    public static IServiceCollection AddThemes(this IServiceCollection services, Action<string, string> setThemeAction)
    {
        return services
            .AddScoped<IThemeLoader, ThemeLoader>()
            .AddSingleton<IThemeSetter>(new ThemeSetter(setThemeAction))
            .AddSingleton<IThemeRepository, ThemeRepository>();
    }

    public static IServiceCollection AddArchiving(this IServiceCollection services)
    {
        var extensions = new List<string>(capacity: 10);
        var zipExtensions = typeof(ZipArchiveReader)
            .GetCustomAttribute<ArchiveExtensionsAttribute>()?
            .Extensions ?? [];
        
        extensions.AddRange(zipExtensions);
        foreach (var extension in extensions)
            services.AddKeyedScoped<ZipArchiveReader>(extension);

        services.AddSingleton<ArchiveReaderFactory>();
        services.AddScoped<IArchiveDirectorySearcher, ArchiveDirectorySearcher>();
        var suppoertedExtensions = new ArchiveSupportedExtensions(extensions.ToArray());
        services.AddSingleton<IArchiveSupportedExtensions>(suppoertedExtensions);
        return services;
    }

    public static IServiceCollection AddExplorer(this IServiceCollection services)
    {
        return services.AddSingleton<IExplorer, Explorer>()
            .AddSingleton<IExplorerHistory, ExplorerHistory>()
            .AddScoped<IDirectorySearcher, DirectorySearcher>()
            .AddSingleton<IExplorerStateMachine, ExplorerStateMachine>();
    }

    public static IServiceCollection AddWpfConverter<T>(this IServiceCollection services) where T : class, IValueConverter
    {
        return services.AddSingleton<T>();
    }
    
    public static IServiceCollection AddWpfMultiConverter<T>(this IServiceCollection services) where T : class, IMultiValueConverter
    {
        return services.AddSingleton<T>();
    }

    public static IServiceCollection AddWindow<T>(this IServiceCollection services, ApplicationViews view) where T : Window
    {
        if (view == ApplicationViews.MainView)
            services.AddKeyedSingleton<Window, T>(view);
        else
            services.AddKeyedTransient<Window, T>(view);
        var viewModelTypeString = typeof(T).FullName?.Replace("View", "ViewModel");
        var viewModelType = Type.GetType(viewModelTypeString ?? string.Empty);
        
        return view == ApplicationViews.MainView 
            ? services.AddSingleton(viewModelType) 
            : services.AddTransient(viewModelType);
    }

   
    
}