namespace SolidZip.Modules.LuaModules;

public sealed class LuaShared(ILogger<LuaShared> logger) : ILuaShared
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public object? Get(string name)
    {
        if (_cache.TryGetValue(name, out var result))
            return result;


        logger.LogWarning("Key {key} not found in shared cache", name);
        return null;
    }

    public void AddOrUpdate(string name, object? value)
    {
        if (value is null)
        {
            logger.LogWarning("Attempted to add null value for key {key}", name);
            return;
        }

        _cache.AddOrUpdate(name, value, (_, _) => value);
    }
}