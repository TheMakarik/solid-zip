namespace SolidZip.Core.Extensions;

public static class ServiceProviderExtensions
{
    public static object GetRequiredService(this IServiceProvider provider, string name)
    {
        var type = Type.GetType(name) ?? AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == name || t.Name == name);

        if (type is null)
            throw new InvalidOperationException($"Type '{name}' not found");

        var service = provider.GetService(type);
 
        if (service is null)
            throw new InvalidOperationException($"Service of type '{name}' not registered in DI container");

        return service;
    }
}