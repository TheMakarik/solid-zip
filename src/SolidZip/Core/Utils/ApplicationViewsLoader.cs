namespace SolidZip.Core.Utils;

public sealed class ApplicationViewsLoader(IServiceProvider services)
{
    public T Load<T>(ApplicationViews view)
    {
        return services.GetKeyedService<T>(view);
    }
}