using SolidZip.Services.WindowsServices;

namespace SolidZip.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExplorer(this IServiceCollection services)
    {
        return services
            .AddSingleton<IExplorer, Explorer>();
    }

    public static IServiceCollection AddProxies(this IServiceCollection services)
    {
        return services
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

    public static IServiceCollection AddIconExtractors(this IServiceCollection services)
    {
        return services
            .AddTransient<IAssociatedIconExtractor, AssociatedIconExtractor>();
    }
}