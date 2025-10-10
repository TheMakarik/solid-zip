using SolidZip.Services.LuaServices;
using SolidZip.Services.LuaServices.Abstraction;
using SolidZip.Services.ProxiesServices;
using SolidZip.Services.ProxiesServices.Abstractions;
using SolidZip.Services.WindowsServices;

namespace SolidZip.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection services)
    {
        return services
            .AddTransient<IExplorerHistory, ExplorerHistory>()
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
            .AddSingleton<IAppDataContentManager, AppDataContentManager>();
    }

    public static IServiceCollection AddFactories(this IServiceCollection services)
    {
        return services
            .AddSingleton<IFileStreamFactory, FileStreamFactory>();
    }

    public static IServiceCollection AddLua(this IServiceCollection services)
    {
        return services
            .AddSingleton<ILuaExtensionsRaiser, LuaExtensionsRaiser>()
            .AddSingleton<LuaFactory>()
            .AddSingleton<ILuaExtensionsLoader, LuaExtensionsLoader>()
            .AddSingleton<ILuaGlobalsLoader, LuaGlobalsLoader>()
            .AddSingleton<ILuaExtensions, LuaExtensions>();
    }

    public static IServiceCollection AddIconExtractors(this IServiceCollection services)
    {
        return services
            .AddTransient<IAssociatedIconExtractor, AssociatedIconExtractor>();
    }
}