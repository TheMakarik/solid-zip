using System.Reflection;
using SolidZip.Services.CompressServices;
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

    public static IServiceCollection AddValidator(this IServiceCollection services)
    {
        return services.AddSingleton<IValidator, GlobalValidator>();
    }
    
    public static IServiceCollection AddIconExtractors(this IServiceCollection services)
    {
        return services
            .AddTransient<IAssociatedIconExtractor, AssociatedIconExtractor>();
    }

    public static IServiceCollection AddArchiveReader<T>(this IServiceCollection services) where T : class, IArchiveReader
    {
        var extension = typeof(T).GetCustomAttribute<ArchiveReaderAttribute>()?.Extension;
        ExceptionHelper.ThrowIf(extension is null, () => new InvalidOperationException($"Cannot add {nameof(T)} because it does not have {nameof(ArchiveReaderAttribute)}"));
        services.AddKeyedScoped<IArchiveReader, T>(extension);
        return services;
    }
    
}