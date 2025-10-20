using SolidZip.Services.Validators;
using SolidZip.Services.Validators.Abstractions;

namespace SolidZip.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection services)
    {
        return services
            .AddSingleton<IExplorerHistory, ExplorerHistory>()
            .AddScoped<IDirectorySearcher, DirectorySearcher>()
            .AddSingleton<IExplorer, Explorer>();
    }

    public static IServiceCollection AddProxies(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPathProxy, PathProxy>()
            .AddSingleton<IFileProxy, FileProxy>()
            .AddSingleton<IDirectoryProxy, DirectoryProxy>();
    }

    public static IServiceCollection AddJsonSerialization(this IServiceCollection services)
    {
        return services
            .AddSingleton<IJsonSerializer, SolidZip.Services.JsonSerializationServices.JsonSerializer>();
    }

    public static IServiceCollection AddAppDataServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IAppDataContentCreator, AppDataContentCreator>()
            .AddScoped<IAppDataContentManager, AppDataContentManager>();
    }

    public static IServiceCollection AddFactories(this IServiceCollection services)
    {
        return services
            .AddTransient<IFileStreamFactory, FileStreamFactory>();
    }

    public static IServiceCollection AddLua(this IServiceCollection services)
    {
        return services
            .AddSingleton<ILuaExtensionsRaiser, LuaExtensionsRaiser>()
            .AddSingleton<LuaFactory>()
            .AddTransient<ILuaExtensionsLoader, LuaExtensionsLoader>()
            .AddSingleton<ILuaGlobalsLoader, LuaGlobalsLoader>()
            .AddSingleton<ILuaExtensions, LuaExtensions>();
    }

    public static IServiceCollection AddValidator<T>(this IServiceCollection services)
    {
        return services.AddSingleton<IValidator, GlobalValidator>();
    }
    
    public static IServiceCollection AddIconExtractors(this IServiceCollection services)
    {
        return services
            .AddTransient<IAssociatedIconExtractor, AssociatedIconExtractor>();
    }
}