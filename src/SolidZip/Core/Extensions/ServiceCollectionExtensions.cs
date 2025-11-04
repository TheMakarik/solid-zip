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
    
    public static IServiceCollection AddPathsCollection(this IServiceCollection services)
    {
        return services.AddSingleton<PathsCollection>();
    }
    
    public static IServiceCollection AddAppData(this IServiceCollection services)
    {
        return services.AddSingleton<IUserJsonCreator, UserJsonCreator>();
    }

    
}