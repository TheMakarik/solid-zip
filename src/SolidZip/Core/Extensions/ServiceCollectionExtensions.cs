namespace SolidZip.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache<T>(this IServiceCollection services) where T : class
    {
        return services.AddSingleton<SharedCache<T>>();
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
            .AddSingleton<ILuaEventRaiser, LuaEventRaiser>()
            .AddSingleton<ILuaUiData, LuaUiData>()
            .AddSingleton<ILuaShared, LuaShared>()
            .AddSingleton<ILuaDebugConsole, LuaDebugConsole>()
            .AddSingleton<ILuaGlobalsLoader, LuaGlobalsLoader>();
    }

    public static IServiceCollection AddWin32(this IServiceCollection services)
    {
        return services
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
        var suppoertedExtensions = new ArchiveSupportedExtensions(extensions.ToArray());
        services.AddSingleton<IArchiveSupportedExtensions>(suppoertedExtensions);
        return services;
    }

    public static IServiceCollection AddExplorer(this IServiceCollection services)
    {
        return services.AddSingleton<IExplorer, Explorer>()
            .AddSingleton<IExplorerHistory, ExplorerHistory>()
            .AddSingleton<IExplorerStateMachine, ExplorerStateMachine>();
    }

    
}